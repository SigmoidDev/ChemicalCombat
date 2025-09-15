using Sigmoid.Upgrading;
using Sigmoid.Reactions;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Rooms
{
    /// <summary>
    /// Indicates that a given object can be hit, although not necessarily destroyed<br/>
    /// (yes, the name is a bit confusing, but i can't think of a better term)
    /// </summary>
    public abstract class DestructibleObject : MonoBehaviour, IDamageSource
    {
        public abstract string DisplayName { get; }
        public abstract DetonatedPool ExplosionPool { get; }
        public virtual void Hit()
        {
            bool hasDestructivePerk = Perks.Has(Perk.Destructive);
            HitMask hitMask = HitMask.Enemies;
            if(!hasDestructivePerk) hitMask |= HitMask.Players;

            Vector2 spawnPosition = transform.position + 0.5f * Vector3.up;
            Explosion explosion = (Explosion) ExplosionPool.Fetch();
            explosion.Initialise(ExplosionPool, spawnPosition, this, hitMask, hasDestructivePerk ? 2f : 1f);
            Destroy(gameObject);
        }
    }
}
