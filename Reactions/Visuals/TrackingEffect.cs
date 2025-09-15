using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// A sprite or collection of sprites that directly follows another transform
    /// </summary>
	public class TrackingEffect : SpawnableEffect<TrackingPool>
	{
        [SerializeField] private Vector2 offset;
		private Transform target;

        public void Track(Transform target) => this.target = target;
        private void Update()
        {
            if(target == null) return;
            transform.position = (Vector2) target.position + offset;
        }

        [SerializeField] private AudioPlayer player;
        public void MakeNoise() => player.Play();
    }
}
