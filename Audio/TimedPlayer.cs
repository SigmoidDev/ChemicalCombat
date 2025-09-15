using UnityEngine;

namespace Sigmoid.Audio
{
	public class TimedPlayer : AudioPlayer
	{
        [SerializeField] private float minInterval;
        [SerializeField] private float maxInterval;

        private float playTimer;
		private void Update()
        {
            if(maxInterval == 0f) return;

            playTimer -= Time.deltaTime;
            if(playTimer <= 0f)
            {
                playTimer = Random.Range(minInterval, maxInterval);
                Play();
            }
        }

        public void Setup(ScriptableAudio clip, Vector2 intervalRange)
        {
            minInterval = intervalRange.x;
            maxInterval = intervalRange.y;
            audioClip = clip;

            if(maxInterval == -1f)
            {
                maxInterval = 0f;
                Play();
            }
        }
	}
}
