using System.Collections;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// Any explosion-type object that effects a group of targets (enemies/players/objects) in a given radius
    /// </summary>
	public abstract class DetonatedEffect : SpawnableEffect<DetonatedPool>
	{
		protected abstract float EffectiveRadius { get; }
		[SerializeField] private LayerMask layerMask;

        public override void Initialise(DetonatedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
			base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            StartCoroutine(Detonate());
        }

		private readonly Collider2D[] buffer = new Collider2D[40];
        private IEnumerator Detonate()
		{
			int numHits = Physics2D.OverlapCircleNonAlloc(transform.position, EffectiveRadius, buffer, layerMask);
			for(int i = 0; i < numHits; i++)
			{
                if(!buffer[i].TryGetComponent(out IAttackable attackable)) continue;
				Effect(attackable);
			}

			yield return OnDetonated();
		}

		protected abstract void Effect(IAttackable attackable);
		protected abstract IEnumerator OnDetonated();
	}
}
