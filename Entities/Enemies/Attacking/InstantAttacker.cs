using System.Collections;
using Sigmoid.Projectiles;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// For ranged attacks that have no travel time and simply deal damage from some effect
    /// </summary>
	public class InstantAttacker : TimedAttacker<InstantParams>
    {
        public InstantAttacker(Enemy enemy, InstantParams parameters) : base(enemy, parameters)
        {
            pool = ProjectileManager.Instance.Get(parameters.Projectile);
            delay = parameters.AttackDelay;
            animation = parameters.AttackAnimation;
            attackSound = me.EnemyType.attackSound;
        }

        private readonly ProjectilePool pool;
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

            HitMask hitMask = target is PlayerAttackable ? HitMask.Players
                            : target is EnemyAttackable ? HitMask.Enemies
                            : HitMask.None;

            if(projectile is Pentacle pentacle)
                pentacle.Initialise(me, pool, hitMask, target.Position);
        }
    }
}
