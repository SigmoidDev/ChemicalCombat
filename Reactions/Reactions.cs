using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// The base class for all reactions, with an update method for interaction with timers
    /// </summary>
	public abstract class Reaction
	{
		protected virtual float BaseDamage => 1f;
		public virtual void Update(float deltaTime){}
		public abstract void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier);
	}

    /// <summary>
    /// A reaction which spawns a physical object that does something for some duration
    /// </summary>
    /// <typeparam name="TPool"></typeparam>
    /// <typeparam name="TSpawn"></typeparam>
    // what in the name of generic god is this
	public abstract class SpawnReaction<TPool, TSpawn> : Reaction where TPool : ObjectPool<TSpawn> where TSpawn : SpawnableEffect<TPool>
	{
		protected float cooldown;
        public override void Update(float deltaTime) => cooldown -= deltaTime;

        public abstract TPool Pool { get; }
		public abstract float Cooldown { get; }

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
			if(cooldown > 0f) return;
			cooldown = Cooldown;

            Pool.Fetch().Initialise(Pool, hit.origin, Player.Instance, HitMask.Enemies | HitMask.Objects, damageMultiplier);
        }
    }

    /// <summary>
    /// Any effect or object which is instantiated by a reaction and will be destroyed
    /// </summary>
    /// <typeparam name="TPool"></typeparam>
	public abstract class SpawnableEffect<TPool> : MonoBehaviour
	{
        protected IDamageSource owner;
        protected HitMask hitMask;
		protected TPool pool;
		public virtual void Initialise(TPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
		{
			this.pool = pool;
            this.owner = owner;
            this.hitMask = hitMask;
			transform.position = point;
		}
	}
}
