using Sigmoid.Enemies;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class AnimatedEffect : SpawnableEffect<VisualPool>
	{
		[SerializeField] private float duration;
        private float elapsed = 0f;

        [SerializeField] private AudioPlayer player;
        public void MakeNoise() => player.Play();

        public override void Initialise(VisualPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            elapsed = 0f;
        }

        private void Update()
        {
            if((elapsed += Time.deltaTime) > duration)
                pool.Release(this);
        }
	}
}
