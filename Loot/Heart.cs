using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Players
{
    /// <summary>
    /// A collectable heart that heals the player for some amount of health
    /// </summary>
	public class Heart : Collectable
	{
		[SerializeField] private ScriptableAudio pickupSound;

        private int healing;
        public Heart Initialise(Vector2 position, int healing)
        {
            Initialise(position);
            this.healing = healing;
            return this;
        }

        protected override void Collect()
        {
            Player.Instance.Heal(healing);
            AudioManager.Instance.Play(pickupSound, transform.position, AudioChannel.Sound);
            Despawn();
        }

        protected override void Despawn() => CollectableSpawner.Instance.HeartPool.Release(this);
	}
}
