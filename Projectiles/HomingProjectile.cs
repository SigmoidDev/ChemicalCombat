using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
    public class HomingProjectile : Projectile
    {
        private IDamageSource owner;
        public override IDamageSource Source => owner;
        private HitMask hitMask;

        private Enemy ownerAsEnemy;
        private bool IsOwnerAlive => ownerAsEnemy == null || ownerAsEnemy.gameObject.activeInHierarchy;

        [SerializeField] private float velocity;
        [SerializeField] private float turning;

        private ProjectilePool pool;
        private IAttackable target;

        public HomingProjectile Initialise(IDamageSource owner, ProjectilePool pool, HitMask hitMask, IAttackable target, float lifetime)
        {
            ownerAsEnemy = owner as Enemy;
            this.owner = owner;
            this.pool = pool;
            this.hitMask = hitMask;
            this.target = target;
            this.lifetime = lifetime;
            elapsed = 0f;
            return this;
        }

        private float lifetime;
        private float elapsed;
        private void Update()
        {
            Home();

            if((elapsed += Time.deltaTime) >= lifetime || !IsOwnerAlive)
                pool.Release(this);
        }

        private void Home()
        {
            Vector2 toTarget = target.Position - (Vector2) transform.position;

            float myAngle = Mathf.Atan2(Body.velocity.y, Body.velocity.x);
            float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x);

            float newAngle = MathsHelper.LerpAngle(myAngle, targetAngle, turning * Time.deltaTime);
            Body.velocity = velocity * new Vector2(
                Mathf.Cos(newAngle),
                Mathf.Sin(newAngle)
            );
        }

        public void Fire(Vector2 origin, Vector2 direction)
        {
			transform.SetPositionAndRotation(origin, Quaternion.identity);
            Body.velocity = direction;
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
                DamageContext context = new DamageContext(1, DamageType.Light, DamageCategory.Blunt, owner);
                player.ReceiveAttack(context);
                pool.Release(this);
            }
            else if(attackable is DamageableAttackable damageable)
            {
                if((hitMask & HitMask.Enemies) != 0)
                {
                    DamageContext context = new DamageContext(8, DamageType.Light, DamageCategory.Blunt, owner);
                    damageable.ReceiveAttack(context);
                    pool.Release(this);
                }
                else if(damageable is EnemyAttackable enemy && enemy.Enemy.DisplayName == owner.DisplayName && elapsed > 0.5f)
                {
                    enemy.Enemy.Die();
                    pool.Release(this);
                }
            }
        }
    }
}
