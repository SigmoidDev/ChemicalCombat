using System.Collections;
using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
    public class Pentacle : Projectile
    {
        private IDamageSource owner;
        public override IDamageSource Source => owner;
        private ProjectilePool pool;
        private HitMask hitMask;
        [SerializeField] private ContactFilter2D filter;

        [SerializeField] private Animator animator;
        [SerializeField] private float safetyPeriod;
        [SerializeField] private float radius;

        public Pentacle Initialise(IDamageSource owner, ProjectilePool pool, HitMask hitMask, Vector2 position)
        {
            this.owner = owner;
            this.pool = pool;
            this.hitMask = hitMask;
            transform.position = position;

            animator.Play("Pentacle");
            StartCoroutine(Curse());
            return this;
        }


        private readonly Collider2D[] buffer = new Collider2D[20];
        private IEnumerator Curse()
        {
            yield return new WaitForSeconds(safetyPeriod);

            int numHits = Physics2D.OverlapCircle(transform.position, radius, filter, buffer);
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out IAttackable attackable)) continue;

                if((hitMask & HitMask.Players) != 0 && attackable is PlayerAttackable player)
                {
                    DamageContext context = new DamageContext(1, DamageType.Fire, DamageCategory.Debuff, Source);
                    player.ReceiveAttack(context);
                }
                if((hitMask & HitMask.Enemies) != 0 && attackable is DamageableAttackable damageable)
                {
                    DamageContext context = new DamageContext(10, DamageType.Fire, DamageCategory.Debuff, Source);
                    damageable.ReceiveAttack(context);
                    damageable.Damageable.DotReceiver.InflictDot(Buffs.DotType.Burning, 3f, 2);
                }
            }

            yield return new WaitForSeconds(1f - safetyPeriod);
            pool.Release(this);
        }

        public override void Collide(Collider2D other){}
        public override void Hit(IAttackable attackable){}
    }
}
