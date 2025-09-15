using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Projectiles
{
    /// <summary>
    /// A gravity-affected projectile that pierces through infinitely many enemies
    /// </summary>
    public class Thorn : Projectile
    {
        [SerializeField] private Vector2Int damageRange;
        [SerializeField] private Vector2 lifetimeRange;
        [SerializeField] private AudioPlayer player;

        public override IDamageSource Source => Player.Instance;
        private ProjectilePool pool;

        [SerializeField] private SpriteRenderer sprite;
        public Thorn Initialise(ProjectilePool pool, Vector2 position, Vector2 force)
        {
            this.pool = pool;
            lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);
			transform.SetPositionAndRotation(position, Quaternion.identity);

            Body.velocity = Vector2.zero;
            Body.totalTorque = 0f;
            Body.AddForce(force, ForceMode2D.Impulse);
            Body.AddTorque(-force.x, ForceMode2D.Impulse);

            player.Play();
            sprite.flipX = force.x < 0;
            return this;
        }

        private float lifetime;
        private void Update()
        {
            lifetime -= Time.deltaTime;
            if(lifetime <= 0f) pool.Release(this);
        }

        public override void Collide(Collider2D other)
        {
            if(other.gameObject.layer != LayerMask.NameToLayer("Ground")
            && other.gameObject.layer != LayerMask.NameToLayer("Furniture")) return;
            pool.Release(this);
        }

        public override void Hit(IAttackable attackable)
        {
            if(attackable is PlayerAttackable or ObjectAttackable) return;

            int damage = Random.Range(damageRange.x, damageRange.y);
            DamageContext context = new DamageContext(damage, DamageType.Physical, DamageCategory.Blunt, Source);
            attackable.ReceiveAttack(context);
        }
    }
}
