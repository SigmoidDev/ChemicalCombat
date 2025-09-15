using System.Collections;
using Sigmoid.Projectiles;
using Sigmoid.Audio;
using UnityEngine;
using Sigmoid.Players;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Enables the spawning of projectiles for ranged attacks with travel time and collision
    /// </summary>
	public class ProjectileAttacker : TimedAttacker<ProjectileParams>
    {
        public ProjectileAttacker(Enemy enemy, ProjectileParams parameters) : base(enemy, parameters)
        {
            pool = ProjectileManager.Instance.Get(parameters.Projectile);
            origin = parameters.SpawnOrigin;
            delay = parameters.AttackDelay;
            animation = parameters.AttackAnimation;
            attackSound = me.EnemyType.attackSound;
        }

        private readonly ProjectilePool pool;
        private readonly Vector2 origin;
        private readonly float delay;
        private readonly string animation;
        private readonly ScriptableAudio attackSound;

        protected IAttackable target;
        public override void Update(IAttackable target, float deltaTime)
        {
            base.Update(target, deltaTime);
            this.target = target;
        }

        public override void Attack() => me.StartCoroutine(CAttack());
        protected virtual IEnumerator CAttack()
        {
            if(me.Velocity.magnitude < 0.01f && target != null)
                me.Sprite.flipX = target.Position.x < me.transform.position.x;

            if(!string.IsNullOrEmpty(animation)) me.Animator.Play(animation);
            if(attackSound) AudioManager.Instance.Play(attackSound, me.transform.position, AudioChannel.Enemy);
            yield return new WaitForSeconds(delay);
            Projectile projectile = pool.Fetch();

            float sideMultiplier = me.Sprite.flipX ? -1f : 1f;
            Vector2 spawnOffset = origin;
            spawnOffset.x *= sideMultiplier;

            Vector2 spawnPosition = (Vector2) me.transform.position + spawnOffset;
            Vector2 direction = GetProjectileDirection(spawnPosition);
            Quaternion rotation = GetProjectileOrientation();

            HitMask hitMask = target is PlayerAttackable ? HitMask.Players
                            : target is EnemyAttackable ? HitMask.Enemies
                            : HitMask.None;

            if(projectile is LinearProjectile linear)
            {
                linear.Initialise(me, pool, hitMask, 3f);
                linear.Throw(spawnPosition, rotation, direction, sideMultiplier);
            }
            else if(projectile is HomingProjectile homing)
            {
                homing.Initialise(me, pool, hitMask, target, 4f);
                homing.Fire(spawnPosition, direction);
            }
        }

        protected virtual Vector2 GetProjectileDirection(Vector2 spawnPosition)
        {
            Vector2 targetPoint = target.Position + 0.1f * target.Velocity;
            return targetPoint - spawnPosition;
        }

        protected virtual Quaternion GetProjectileOrientation() => Quaternion.identity;
    }
}
