using System.Collections;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Simply uses an OverlapCircle to check for attackable colliders in a given range
    /// </summary>
    public class MeleeAttacker : TimedAttacker<MeleeParams>
    {
        public MeleeAttacker(Enemy enemy, MeleeParams parameters) : base(enemy, parameters)
        {
            attackSound = me.EnemyType.attackSound;
            animation = parameters.AttackAnimation;
            delay = parameters.AttackDelay;
            damageType = parameters.DamageType;
            offset = parameters.AttackOrigin;
            area = parameters.AttackArea;
            timer = 0f;

            filter = new ContactFilter2D();
            filter.SetLayerMask(me.HitboxMask);
        }

        private readonly ScriptableAudio attackSound;
        private readonly string animation;
        private readonly float delay;
        private readonly DamageType damageType;
        private readonly Vector2 offset;
        private readonly Vector2 area;

        private readonly Collider2D[] buffer = new Collider2D[20];
        private ContactFilter2D filter;

        public override void Attack() => me.StartCoroutine(CAttack());
        protected virtual float GetDamage(IAttackable attackable)
        {
            if(attackable is PlayerAttackable) return 1f;
            return 5f;
        }

        private IEnumerator CAttack()
        {
            if(!string.IsNullOrEmpty(animation)) me.Animator.Play(animation);
            if(attackSound) AudioManager.Instance.Play(attackSound, me.transform.position, AudioChannel.Enemy);
            yield return new WaitForSeconds(delay);

            Vector2 origin = offset;
            origin.x *= me.Sprite.flipX ? -1f : 1f;
            origin += (Vector2) me.transform.position;

            int numHits = Physics2D.OverlapCapsule(origin, area, CapsuleDirection2D.Horizontal, 0f, filter, buffer);            
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out IAttackable attackable)
                || attackable is PlayerAttackable && me.Target is not PlayerAttackable
                || attackable is EnemyAttackable enemy && (me.Target is not EnemyAttackable
                || enemy.Enemy == me)) continue;

                float damage = GetDamage(attackable);
                DamageContext context = new DamageContext(damage, damageType, DamageCategory.Blunt, me);
                attackable.ReceiveAttack(context);
            }
        }
    }
}
