using System;
using Sigmoid.Projectiles;
using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Effects;
using Sigmoid.Buffs;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// A base for any reaction which has some custom effect with a tracking sprite (such as freeze and levitate)
    /// </summary>
    public abstract class CustomDebuffReaction : SpawnReaction<TrackingPool, TrackingEffect>
    {
        public abstract float Duration { get; }
        public abstract Guid Apply(DamageableAttackable source);
        public abstract void Remove(DamageableAttackable source, Guid guid);

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(source.Damageable.IsDead) return;

            BuffReceiver receiver = source.Damageable.BuffReceiver;
            TimedModifier modifier = receiver.GetStoredModifier(this);
            if(modifier == null)
            {
                TrackingEffect effect = Pool.Fetch();
                effect.Initialise(Pool, source.Damageable.transform.position, Player.Instance, HitMask.Enemies | HitMask.Objects, damageMultiplier);
                effect.Track(source.Damageable.transform);
                effect.MakeNoise();

                void Delete()
                {
                    Pool.Release(effect);
                    source.Damageable.OnDeath -= DeleteOnDeath;
                    SceneLoader.Instance.OnSceneUnloading -= DeleteOnUnload;
                }
                void DeleteOnDeath() => Delete();
                source.Damageable.OnDeath += DeleteOnDeath;
                void DeleteOnUnload(GameScene scene) => Delete();
                SceneLoader.Instance.OnSceneUnloading += DeleteOnUnload;

                if(source.Damageable.TryGetComponent(out Enemy enemy))
                {
                    enemy.Movement.IsEnabled = false;
                    enemy.Targeting.IsEnabled = false;
                    enemy.Attacker.IsEnabled = false;
                }

                modifier = new TimedModifier(
                    Apply(source),
                    guid =>
                    {
                        Remove(source, guid);
                        Pool.Release(effect);
                        if(enemy)
                        {
                            enemy.Movement.IsEnabled = true;
                            enemy.Targeting.IsEnabled = true;
                            enemy.Attacker.IsEnabled = true;
                        }
                    }
                );
            }
            
            modifier.timer = Duration;
            receiver.Apply(this, modifier);
        }
    }

    /// <summary>
    /// Makes enemies drunk for 8s, causing them to aimlessly wander around
    /// </summary>
    public class AlcoholReaction : SpawnReaction<VisualPool, AnimatedEffect>
    {
        public override VisualPool Pool => ReactionPool.Instance.ConfusedPool;
        public override float Cooldown => 0.5f;
        public float Duration => 8.0f * PlayerStats.EffectDuration;

        private readonly Collider2D[] buffer;
        private readonly ContactFilter2D filter;

        public AlcoholReaction()
        {
            buffer = new Collider2D[10];
            filter = new ContactFilter2D();
            filter.layerMask = LayerMask.NameToLayer("Hitboxes");
        }

        /// <summary>
        /// Overrides the target and targeting
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="drunkTargeting"></param>
        /// <returns></returns>
        private Guid MakeDrunk(Enemy enemy, ITargeting drunkTargeting)
        {
            enemy.SetTarget(enemy.Attackable);
            return enemy.OverrideTargeting(drunkTargeting);
        }

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            int numHits = Physics2D.OverlapCircle(source.transform.position, 1f, filter, buffer);
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out EnemyAttackable enemy)) continue;

                DamageContext context = new DamageContext(3 * damageMultiplier, DamageType.Acidic, DamageCategory.Blunt, Player.Instance);
                enemy.ReceiveAttack(context);

                ITargeting drunkTargeting = ReactionPool.Instance.DrunkParams.CreateModule(enemy.Enemy);
                drunkTargeting.Initialise();

                BuffReceiver receiver = enemy.Damageable.BuffReceiver;
                TimedModifier modifier = receiver.GetStoredModifier(this);
                modifier ??= new TimedModifier(
                    MakeDrunk(enemy.Enemy, drunkTargeting),
                    _ =>
                    {
                        drunkTargeting.Destroy();
                        enemy.Enemy.OverrideTargeting(null);
                        enemy.Enemy.SetTarget(null);
                    }
                );
                modifier.timer = Duration;
                receiver.Apply(this, modifier);
            }

            base.React(source, hit, damageMultiplier);
        }
    }

    /// <summary>
    /// Deals a small amount of damage and freezes enemies in a block of ice for 4s
    /// </summary>
    public class FreezeReaction : CustomDebuffReaction
    {
        public override TrackingPool Pool => ReactionPool.Instance.IcePool;
        public override float Cooldown => 0.0f;
        public override float Duration => 4.0f * PlayerStats.EffectDuration;

        public override Guid Apply(DamageableAttackable source) => source.Damageable.BuffReceiver.MoveSpeed.AddModifier(speed => speed * 0f);
        public override void Remove(DamageableAttackable source, Guid guid) => source.Damageable.BuffReceiver.MoveSpeed.RemoveModifier(guid);

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            base.React(source, hit, damageMultiplier);
            DamageContext context = new DamageContext(6 * damageMultiplier, DamageType.Glacial, DamageCategory.Blunt, Player.Instance);
            source.ReceiveAttack(context);
        }
    }

    /// <summary>
    /// Deals medium damage and makes the target glow, increasing their damage taken by 50% for 6s
    /// </summary>
    public class GlowingReaction : Reaction
    {
        public float Duration => 6.0f * PlayerStats.EffectDuration;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            SpriteRenderer sprite = source.Damageable.TryGetComponent(out Enemy enemy) ? enemy.Sprite
                                  : source.Damageable.TryGetComponent(out TargetDummy dummy) ? dummy.Sprite : null;
            if(sprite == null) return;
            sprite.material = MaterialManager.GlowingMaterial;

            BuffReceiver receiver = source.Damageable.BuffReceiver;
            TimedModifier modifier = receiver.GetStoredModifier(this);
            modifier ??= new TimedModifier(
                receiver.DamageTaken.AddModifier(damage => damage * 1.5f),
                guid =>
                {
                    receiver.DamageTaken.RemoveModifier(guid);
                    sprite.material = MaterialManager.ReplacementMaterial;
                }
            );
            modifier.timer = Duration;
            receiver.Apply(this, modifier);

            //Deal damage at the end so that it is increased by the buff
            DamageContext context = new DamageContext(4 * damageMultiplier, DamageType.Light, DamageCategory.Blunt, Player.Instance);
            source.ReceiveAttack(context);
        }
    }

    /// <summary>
    /// Deals a small amount of damage and causes enemies to fly upwards uncontrollably for 2.5s
    /// </summary>
    public class LevitateReaction : CustomDebuffReaction
    {
        public override TrackingPool Pool => ReactionPool.Instance.BalloonPool;
        public override float Cooldown => 0.0f;
        public override float Duration => 2.5f * PlayerStats.EffectDuration;

        public override Guid Apply(DamageableAttackable source)
        {
            if(source.Damageable.TryGetComponent(out Enemy enemy))
            {
                enemy.Body.velocity = enemy.Agent.velocity;
                enemy.Body.drag = 2f;
                enemy.Body.gravityScale = -1f;
            }

            return source.Damageable.BuffReceiver.MoveSpeed.AddModifier(speed => speed * 0f);
        }

        public override void Remove(DamageableAttackable source, Guid guid)
        {
            source.Damageable.BuffReceiver.MoveSpeed.RemoveModifier(guid);
            if(source.Damageable.TryGetComponent(out Enemy enemy))
            {
                enemy.Body.drag = 10f;
                enemy.Body.gravityScale = 0f;
            }
        }

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            base.React(source, hit, damageMultiplier);
            DamageContext context = new DamageContext(7 * damageMultiplier, DamageType.Breeze, DamageCategory.Blunt, Player.Instance);
            source.ReceiveAttack(context);
        }
    }

    /// <summary>
    /// Deals minor damage and has a 33% chance to make non-boss enemies fight each other to the death
    /// </summary>
	public class SaltReaction : SpawnReaction<VisualPool, AnimatedEffect>
    {
        public override VisualPool Pool => ReactionPool.Instance.RagePool;
        public override float Cooldown => 0.0f;
        public float Duration => 8.0f * PlayerStats.EffectDuration;

        private readonly Collider2D[] buffer;
        private ContactFilter2D enemyFilter;

        public SaltReaction()
        {
            buffer = new Collider2D[12];
            enemyFilter.layerMask = LayerMask.NameToLayer("Hitboxes");
        }

        private Guid AttackOthers(DamageableAttackable source)
        {
            if(!source.Damageable.TryGetComponent(out Enemy enemy)) return Guid.NewGuid();

            EnemyAttackable target = null;
            float minDistance = Mathf.Infinity;

            int numHits = Physics2D.OverlapCircle(source.Damageable.transform.position, 16f, enemyFilter, buffer);
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out EnemyAttackable other) || other.Enemy == enemy) continue;

                float distance = Vector2.Distance(enemy.transform.position, other.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    target = other;
                }
            }

            enemy.SetTarget(target);
            return Guid.NewGuid();
        }

        private void ReturnToNormal(DamageableAttackable source)
        {
            if(source.Damageable.TryGetComponent(out Enemy enemy))
                enemy.SetTarget(Player.Instance.Attackable);
        }

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            DamageContext context = new DamageContext(2 * damageMultiplier, DamageType.Physical, DamageCategory.Blunt, Player.Instance);
            source.ReceiveAttack(context);

            if(cooldown > 0f) return;
            base.React(source, hit, damageMultiplier);

            BuffReceiver receiver = source.Damageable.BuffReceiver;
            TimedModifier modifier = receiver.GetStoredModifier(this);
            modifier ??= new TimedModifier(
                AttackOthers(source),
                guid => ReturnToNormal(source)
            );
            modifier.timer = Duration;
            receiver.Apply(this, modifier);
        }
    }

    /// <summary>
    /// Does no damage, but reduces enemy movement speed by 75% for 8s
    /// </summary>
    public class WaterReaction : SpawnReaction<VisualPool, AnimatedEffect>
    {
        public override VisualPool Pool => ReactionPool.Instance.SplashPool;
        public override float Cooldown => 0.0f;
        public float Duration => 8.0f * PlayerStats.EffectDuration;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            DamageContext context = new DamageContext(3 * damageMultiplier, DamageType.Glacial, DamageCategory.Blunt, Player.Instance);
            source.ReceiveAttack(context);

            BuffReceiver receiver = source.Damageable.BuffReceiver;
            TimedModifier modifier = receiver.GetStoredModifier(this);
            modifier ??= new TimedModifier(
                receiver.MoveSpeed.AddModifier(speed => speed * 0.25f),
                guid => receiver.MoveSpeed.RemoveModifier(guid)
            );
            modifier.timer = Duration;
            receiver.Apply(this, modifier);

            base.React(source, hit, damageMultiplier);
        }
    }
}
