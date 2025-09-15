using System.Collections;
using System;
using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine.AI;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Handles all of the AI for an enemy (by means of an IMovement, ITargeting, and IAttacker)
    /// </summary>
	public class Enemy : MonoBehaviour, IDamageSource
	{
		[field: SerializeField] public Damageable Damageable { get; private set; }
		[field: SerializeField] public EnemyAttackable Attackable { get; private set; }
		[field: SerializeField] public Animator Animator { get; private set; }
		[field: SerializeField] public NavMeshAgent Agent { get; private set; }
		[field: SerializeField] public SpriteRenderer Sprite { get; private set; }
		[field: SerializeField] public SpriteRenderer Shadow { get; private set; }
		[field: SerializeField] public CapsuleCollider2D Collider { get; private set; }
		[field: SerializeField] public BoxCollider2D Hitbox { get; private set; }
		[field: SerializeField] public Rigidbody2D Body { get; private set; }
		[field: SerializeField] public LayerMask HitboxMask { get; private set; }
		[field: SerializeField] public TimedPlayer TimedAudio { get; private set; }
        [field: SerializeField] public EnemyFlipper Flipper { get; private set; }
        [field: SerializeField] public PaletteSwapper Swapper { get; private set; }

        public string DisplayName => EnemyType.name;
        public Vector2 Velocity
        {
            get => Agent.enabled ? Agent.velocity : Body.velocity;
            set
            {
                if(Agent.enabled) Agent.velocity = value;
                else Body.velocity = value;
            }
        }

        public Stat<float> SpeedMultiplier => Damageable.BuffReceiver.MoveSpeed;
        public ScriptableEnemy EnemyType { get; private set; }

        private IMovement movement;
        private IMovement movementOverride;
        public IMovement BaseMovement => movement;
        public IMovement Movement => movementOverride ?? movement;

        private ITargeting targeting;
        private ITargeting targetingOverride;
        public ITargeting BaseTargeting => targeting;
        public ITargeting Targeting => targetingOverride ?? targeting;

        private IAttacker attacker;
        private IAttacker attackerOverride;
        public IAttacker BaseAttacker => attacker;
        public IAttacker Attacker => attackerOverride ?? attacker;

        public event Action<Enemy> OnDeath;

        private void Awake()
        {
            //i don't know why these properties aren't just serialised but anyway
            Agent.updateRotation = false;
			Agent.updateUpAxis = false;

            Damageable.OnDeath += Die; //unsubscribed automatically on scene unload
            Damageable.OnDamage += MakeNoise; //unsubscribed automatically on scene unload
        }

        /// <summary>
        /// Initialises a new enemy at the given position and adds components based on its type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
		public Enemy Initialise(ScriptableEnemy type, Vector2 position)
        {
            EnemyType = type;
            gameObject.name = type.name;

            MoveTo(position);
            Agent.enabled = true;
            Agent.acceleration = type.acceleration.GetAccelerationValue();
            Agent.angularSpeed = type.turning.GetTurningValue();

            Shadow.sprite = type.shadow;
            Shadow.enabled = true;
            Flipper.enabled = true;
            Sprite.material = MaterialManager.ReplacementMaterial;
            SetAlpha(1f);

            Animator.runtimeAnimatorController = type.animator;
            TimedAudio.Setup(type.randomSound, type.soundInterval);

            ScriptableHitbox hitbox = type.hitbox;
            Collider.offset = hitbox.colliderCentre;
            Collider.size = hitbox.colliderSize;
            Agent.radius = hitbox.pathfinderSize.x;
            Agent.height = hitbox.pathfinderSize.y;
            Hitbox.offset = hitbox.hitboxCentre;
            Hitbox.size = hitbox.hitboxSize;
            Body.mass = type.mass;
            Body.gravityScale = 0f;

            int maxHealth = (int) (type.health * DifficultyManager.HealthMultipler * DifficultyManager.GetHealthScaling());
            Damageable.Initialise(maxHealth);
            Damageable.DotReceiver.ResetDoTs(type);
            Damageable.BuffReceiver.Initialise();

            movement = type.movement.CreateModule(this);
            targeting = type.targeting.CreateModule(this);
            attacker = type.attacker.CreateModule(this);
            movement.Initialise();
            targeting.Initialise();
            attacker.Initialise();

            movementOverride = null;
            targetingOverride = null;
            attackerOverride = null;
            SetTarget(null);

            alwaysUpdate = type.alwaysUpdate;
            return this;
        }

        public void MoveTo(Vector2 position)
        {
            transform.position = position;
            Agent.Warp(position);
        }

        /// <summary>
        /// Sets the alphas 
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            Color colour = Sprite.color;
            colour.a = alpha;
            Sprite.color = colour;

            Color shadow = Shadow.color;
            shadow.a = alpha * 0.5f;
            Shadow.color = shadow;

            Damageable.ChemicalReactor.StatusBelt.SetAlpha(alpha);
        }

        /// <summary>
        /// Plays a damage noise (if set) upon taking damage
        /// </summary>
        /// <param name="damage"></param>
        private void MakeNoise(int damage){ if(EnemyType.hurtSound) AudioManager.Instance.Play(EnemyType.hurtSound, transform.position, AudioChannel.Enemy); }

        /// <summary>
        /// Plays the unused sound on the ScriptableEnemy type
        /// </summary>
        public void PlayOtherSound(){ if(EnemyType.otherSound) AudioManager.Instance.Play(EnemyType.otherSound, transform.position, AudioChannel.Enemy); }

        /// <summary>
        /// Properly destroys the enemy on death, triggering Destroy on all 3 AI modules
        /// </summary>
        public void Die()
        {
            if(isDying) return;
            StartCoroutine(CDie());
        }

        private bool isDying;
        private IEnumerator CDie()
        {
            isDying = true;
            BaseMovement.Destroy();
            BaseTargeting.Destroy();
            BaseAttacker.Destroy();
            yield return BaseAttacker.Kill();
            isDying = false;

            OnDeath?.Invoke(this);
            if(EnemyType.deathSound) AudioManager.Instance.Play(EnemyType.deathSound, transform.position, AudioChannel.Enemy);
            CollectableSpawner.Instance.DropCoins(transform.position, EnemyType.NumCoins + PlayerStats.BonusCoins);
            EnemySpawner.Instance.KillEnemy(this);
        }

        /// <summary>
        /// Enables all 3 AI modules
        /// </summary>
        public void ForceReenable()
        {
            Movement.IsEnabled = true;
            Targeting.IsEnabled = true;
            Attacker.IsEnabled = true;
        }

        /// <summary>
        /// Disables all 3 AI modules
        /// </summary>
        public void ForceStop()
        {
            Movement.IsEnabled = false;
            Targeting.IsEnabled = false;
            Attacker.IsEnabled = false;
        }

        /// <summary>
        /// Temporarily overrides the movement module for debuffing purposes
        /// </summary>
        /// <param name="temporary"></param>
        /// <returns></returns>
        public Guid OverrideMovement(IMovement temporary)
        {
            movementOverride = temporary;
            return Guid.NewGuid();
        }

        /// <summary>
        /// Temporarily overrides the targeting module for debuffing purposes
        /// </summary>
        /// <param name="temporary"></param>
        /// <returns></returns>
        public Guid OverrideTargeting(ITargeting temporary)
        {
            targetingOverride = temporary;
            return Guid.NewGuid();
        }

        /// <summary>
        /// Temporarily overrides the attacking module for debuffing purposes
        /// </summary>
        /// <param name="temporary"></param>
        /// <returns></returns>
        public Guid OverrideAttacker(IAttacker temporary)
        {
            attackerOverride = temporary;
            return Guid.NewGuid();
        }

        /// <summary>
        /// Disables the Agent and allows forces to be applied to the Rigidbody for a given duration
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="temporaryDrag"></param>
        public void PermitForces(float duration, float temporaryDrag)
        {
            if(activeForcePermitter != null) StopCoroutine(activeForcePermitter);
            activeForcePermitter = StartCoroutine(CPermitForces(duration, temporaryDrag));
        }
        private Coroutine activeForcePermitter;
        private IEnumerator CPermitForces(float duration, float temporaryDrag)
        {
            Agent.enabled = false;
            Body.drag = temporaryDrag;
            yield return new WaitForSeconds(duration);
            Body.drag = 10f;
            Agent.enabled = true;
        }

        private IAttackable target;
        public IAttackable Target => target ??= Player.Instance.Attackable;
        public void SetTarget(IAttackable attackable) => target = attackable;

        private Vector2 targetPosition;
        public bool AtDestination { get; private set; }
        private bool alwaysUpdate;

        private void Update()
        {
            if(Targeting.IsEnabled) AtDestination = Targeting.GetDestination(Target, out targetPosition);
            if(Movement.IsEnabled && (!AtDestination || alwaysUpdate)) Movement.Update(targetPosition, Time.deltaTime);
            if(Attacker.IsEnabled) Attacker.Update(Target, Time.deltaTime);
        }
	}
}
