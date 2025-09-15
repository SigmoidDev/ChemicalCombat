using Sigmoid.Reactions;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
	public class Bomb : Projectile
	{
		private IDamageSource owner;
        public override IDamageSource Source => owner;
        private ProjectilePool pool;
        private HitMask hitMask;

        [SerializeField] private float detonationTime;
        [SerializeField] private float radius;
        [SerializeField] private SpriteRenderer flashSprite;
        [SerializeField] private AnimationCurve flashCurve;

        public Bomb Initialise(IDamageSource owner, ProjectilePool pool, HitMask hitMask, Vector2 position)
        {
            this.owner = owner;
            this.pool = pool;
            this.hitMask = hitMask;
            transform.position = position;
            elapsed = 0f;
            return this;
        }

        private float elapsed;
        private void Update()
        {
            float value = flashCurve.Evaluate(elapsed / detonationTime);
            flashSprite.color = new Color(1f, 1f, 1f, value);

            if((elapsed += Time.deltaTime) >= detonationTime)
            {
                elapsed = 0f;
                Explode();
            }
        }

        private void Explode()
        {
            DetonatedPool explosionPool = ReactionPool.Instance.FusionPool;
            explosionPool.Fetch().Initialise(explosionPool, transform.position, owner, hitMask, 1f);
            pool.Release(this);
        }

        public override void Collide(Collider2D other){}
        public override void Hit(IAttackable attackable){}
    }
}
