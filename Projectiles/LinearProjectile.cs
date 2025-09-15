using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
    public class LinearProjectile : Projectile
    {
        [SerializeField] private float startVelocity;
        [SerializeField] private float startTorque;
        [SerializeField] private int damage;
        [SerializeField] private int piercing;

        private IDamageSource owner;
        public override IDamageSource Source => owner;
        private HitMask hitMask;

        private ProjectilePool pool;
        public LinearProjectile Initialise(IDamageSource owner, ProjectilePool pool, HitMask hitMask, float lifetime)
        {
            this.owner = owner;
            this.pool = pool;
            this.hitMask = hitMask;
            this.lifetime = lifetime;
            elapsed = 0f;
            hits = piercing;
            return this;
        }

        private float lifetime;
        private float elapsed;
        private int hits;
        private void Update()
        {
            if((elapsed += Time.deltaTime) >= lifetime)
                pool.Release(this);
        }

        public void Throw(Vector2 origin, Quaternion rotation, Vector2 direction, float angularMultiplier)
        {
			transform.SetPositionAndRotation(origin, rotation);
            Body.velocity = startVelocity * direction.normalized;
            Body.AddTorque(startTorque * angularMultiplier, ForceMode2D.Impulse);
        }

        public override void Collide(Collider2D other)
        {
            if(other.gameObject.layer != LayerMask.NameToLayer("Ground")
            && other.gameObject.layer != LayerMask.NameToLayer("Furniture")) return;
            pool.Release(this);
        }

        public override void Hit(IAttackable attackable)
        {
            if((hitMask & HitMask.Players) != 0 && attackable is PlayerAttackable player)
            {
                DamageContext context = new DamageContext(1, DamageType.Physical, DamageCategory.Blunt, owner);
                player.ReceiveAttack(context);
                if(--hits <= 0) pool.Release(this);
            }
            else if((hitMask & HitMask.Enemies) != 0 && attackable is DamageableAttackable damageable)
            {
                if(damageable is EnemyAttackable enemy && enemy.Enemy.Equals(owner)) return;

                DamageContext context = new DamageContext(damage, DamageType.Physical, DamageCategory.Blunt, owner);
                damageable.ReceiveAttack(context);
                if(--hits <= 0) pool.Release(this);
            }
        }
    }
}
