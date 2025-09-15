using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
	public abstract class Projectile : MonoBehaviour
	{
        public abstract IDamageSource Source { get; }
		[field: SerializeField] public Rigidbody2D Body { get; private set; }

        public abstract void Collide(Collider2D other);
		public abstract void Hit(IAttackable attackable);

        private void OnTriggerEnter2D(Collider2D other)
		{
            if(other.gameObject.layer == LayerMask.NameToLayer("Hitboxes")
            && other.gameObject.TryGetComponent(out IAttackable attackable))
                Hit(attackable);

            else Collide(other);
		}
	}

	public readonly struct ProjectileHit
	{
        public readonly Projectile projectile;
		public readonly Vector2 origin;
		public readonly Vector2 point;
		public readonly Vector2 velocity;

		public ProjectileHit(Projectile projectile, Vector2 origin, Vector2 point, Vector2 velocity)
		{
            this.projectile = projectile;
			this.origin = origin;
			this.point = point;
			this.velocity = velocity;
		}
	}
}
