using System.Collections;
using Sigmoid.Enemies;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// An object which will remain stationary and do something on enter/exit rather than on creation
    /// </summary>
	public abstract class SpawnedEffect : SpawnableEffect<SpawnedPool>
	{
        [SerializeField] protected AudioPlayer audioPlayer;

		public abstract void OnEnter(DamageableAttackable damageable);
		public abstract void OnExit(DamageableAttackable damageable);

		protected abstract float Lifetime { get; }
		public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
			base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            audioPlayer.Play();
            StartCoroutine(Kill());
        }
		protected IEnumerator Kill()
		{
			yield return new WaitForSeconds(Lifetime * PlayerStats.EffectDuration);
			pool.Release(this);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if(other.gameObject.layer != LayerMask.NameToLayer("Hitboxes")
			|| !other.TryGetComponent(out DamageableAttackable attackable)) return;
			OnEnter(attackable);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if(other.gameObject.layer != LayerMask.NameToLayer("Hitboxes")
			|| !other.TryGetComponent(out DamageableAttackable attackable)) return;
			OnExit(attackable);
		}
	}
}
