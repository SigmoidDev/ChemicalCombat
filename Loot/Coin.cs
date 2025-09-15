using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// A collectable coin that gives the player some amount of money
    /// </summary>
    public class Coin : Collectable
    {
        [SerializeField] private ScriptableAudio pickupSound;

        private int value;
        public Coin Initialise(Vector2 position, int value)
        {
            Initialise(position);
            this.value = value;
            return this;
        }

        protected override void Collect()
        {
            CoinManager.Earn(value);
            AudioManager.Instance.Play(pickupSound, transform.position, AudioChannel.Sound);
            Despawn();
        }

        protected override void Despawn() => CollectableSpawner.Instance.CoinPool.Release(this);
    }
}
