using System.Collections;
using System;
using Sigmoid.Projectiles;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Rooms;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Upgrading
{
    /// <summary>
    /// Handles all event-based perks (i.e. those that are not simple stat increases)
    /// </summary>
	public class PerkHandler : Singleton<PerkHandler>
	{
        private static WaitForSeconds _waitForSeconds6 = new WaitForSeconds(6f);
        [SerializeField] private LayerMask enemyMask;

        private void Awake() => SceneLoader.Instance.OnSceneLoaded += RegisterSceneSpecificEvents;
        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RegisterSceneSpecificEvents;
        }

        private bool alreadyLoadedOnce;
        private void RegisterSceneSpecificEvents(GameScene scene)
        {
            vampiricKills = 0;
            if(!alreadyLoadedOnce)
            {
                Player.Instance.OnHealthChanged += Check1HP; //unsubscribed automatically on unload of Player scene
                Player.Instance.OnHit += OnTakeDamage; //same as above
                alreadyLoadedOnce = true;
            }

            if(scene == GameScene.Menu) return; //i.e. performs the following in Home and Labyrinth
            EnemySpawner.Instance.OnEnemySpawned += AddEnemyTriggers; //unsubscribed automatically on scene switch

            if(scene != GameScene.Labyrinth) return; //i.e. performs the following only in Labyrinth
            RoomGetter.Instance.OnRoomEntered += EnterRoom; //unsubscribed automatically on scene switch
            RoomGetter.Instance.OnRoomCleared += ClearRoom; //same as above
        }

        private void AddEnemyTriggers(Enemy enemy)
        {
            if(Perks.Has(Perk.Contagious)) enemy.OnDeath += Contaminate;
            enemy.OnDeath += OnKill;
        }

        private void OnKill(Enemy enemy)
        {
            if(Perks.Has(Perk.Vampiric)) AddVampireKill(enemy);
            if(Perks.Has(Perk.Renewable)) AddComboKill(enemy);
        }

        #region Contagious
        private readonly Collider2D[] buffer = new Collider2D[40];
        /// <summary>
        /// Spreads DoTs to all enemies in the vicinity (called on death from the Contagious perk)
        /// </summary>
        /// <param name="enemy"></param>
        private void Contaminate(Enemy enemy)
        {
            enemy.OnDeath -= Contaminate;
            int numHits = Physics2D.OverlapCircleNonAlloc(enemy.transform.position, 1f, buffer, enemyMask);
            for(int i = 0; i < numHits; i++)
            {
                if(buffer[i].gameObject.layer != LayerMask.NameToLayer("Hitboxes")) continue;
                if(!buffer[i].TryGetComponent(out DamageableAttackable attackable)) return;

                DotReceiver receiver = attackable.Damageable.DotReceiver;
                receiver.ForEach((dot, status) =>
                {
                    if(status.stacks <= 0) return;
                    receiver.InflictDot(dot, 1f, 1);
                });
            }
        }
        #endregion

        #region Adrenalised
        private Guid adrenalisedReloadModifier;
        private Guid adrenalisedThrowModifier;

        private void Check1HP(int health)
        {
            if(!Perks.Has(Perk.Adrenalised)) return;

            if(health == 1)
            {
                adrenalisedReloadModifier = PlayerStats.ReloadSpeed.AddModifier(time => time * 0.5f);
                adrenalisedThrowModifier = PlayerStats.ThrowRate.AddModifier(rate => rate * 0.5f);
            }
            else
            {
                if(adrenalisedReloadModifier != Guid.Empty) adrenalisedReloadModifier = PlayerStats.ReloadSpeed.RemoveModifier(adrenalisedReloadModifier);
                if(adrenalisedThrowModifier != Guid.Empty) adrenalisedThrowModifier = PlayerStats.ThrowRate.RemoveModifier(adrenalisedThrowModifier);
            }
        }
        #endregion

        #region Exhilarated
        private Guid exhilaratedMoveModifier;
        private Guid exhilaratedThrowModifier;
        private Guid exhilaratedReloadModifier;

        private void EnterRoom(PhysicalRoom room)
        {
            if(room.Room.type.IsCustom() || !Perks.Has(Perk.Exhilarated)) return;

            if(exhilaratedMoveModifier == Guid.Empty) exhilaratedMoveModifier = PlayerStats.MoveSpeed.AddModifier(speed => speed + 50f);
            if(exhilaratedThrowModifier == Guid.Empty) exhilaratedThrowModifier = PlayerStats.ThrowRate.AddModifier(rate => rate * 0.667f);
            if(exhilaratedReloadModifier == Guid.Empty) exhilaratedReloadModifier = PlayerStats.ReloadSpeed.AddModifier(rate => rate * 0.667f);

            if(activeExhilaratedRemoval != null) StopCoroutine(activeExhilaratedRemoval);
            activeExhilaratedRemoval = StartCoroutine(RemoveExhilaratedBuff());
        }

        private Coroutine activeExhilaratedRemoval;
        private IEnumerator RemoveExhilaratedBuff()
        {
            yield return _waitForSeconds6;
            if(exhilaratedMoveModifier != Guid.Empty) exhilaratedMoveModifier = PlayerStats.MoveSpeed.RemoveModifier(exhilaratedMoveModifier);
            if(exhilaratedThrowModifier != Guid.Empty) exhilaratedThrowModifier = PlayerStats.ThrowRate.RemoveModifier(exhilaratedThrowModifier);
            if(exhilaratedReloadModifier != Guid.Empty) exhilaratedReloadModifier = PlayerStats.ReloadSpeed.RemoveModifier(exhilaratedReloadModifier);
        }
        #endregion

        #region Rejuvenating
        private void ClearRoom(PhysicalRoom room)
        {
            if(!Perks.Has(Perk.Rejuvenating) || room is not EnemyRoom) return;
            Player.Instance.Heal();
        }
        #endregion

        #region Renewable
        private int currentCombo;
        private float comboTimer;
        public float ComboBonus => 1f + 0.05f * Math.Min(5, currentCombo);

        private void AddComboKill(Enemy enemy)
        {
            currentCombo++;
            comboTimer = 3f;
        }

        private void Update()
        {
            if(currentCombo == 0) return;
            if((comboTimer -= Time.deltaTime) < 0f)
                currentCombo = 0;
        }
        #endregion

        #region Vampiric
        private int vampiricKills;
        private void AddVampireKill(Enemy enemy)
        {
            if(vampiricKills++ < 40) return;

            vampiricKills -= 40;
            Player.Instance.Heal();
        }
        #endregion

        #region Thorny
        [SerializeField] private Projectile thornProjectile;
        private ProjectilePool thornPool;
        public ProjectilePool ThornPool => thornPool != null ? thornPool : thornPool = ProjectileManager.Instance.Get(thornProjectile);

        private void OnTakeDamage(DamageContext context, bool dodged)
        {
            if(!Perks.Has(Perk.Thorny)) return;

            for(int i = 0; i < 6; i++)
            {
                Thorn thorn = (Thorn) ThornPool.Fetch();
                Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(3f, 4f));
                thorn.Initialise(ThornPool, Player.Instance.transform.position + 0.5f * Vector3.up, randomForce);
            }
        }
        #endregion
    }
}
