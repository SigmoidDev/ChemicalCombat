using System.Collections;
using System;
using UnityEngine;

namespace Sigmoid.Audio
{
    /// <summary>
    /// Extends AudioPlayer to allow fading in/out, as well as looping after a random interval
    /// </summary>
	public class MusicPlayer : AudioPlayer
	{
        /// <summary>
        /// Fades linearly from the current volume to the desired volume over a set time period
        /// </summary>
        /// <param name="to"></param>
        /// <param name="over"></param>
        public void Fade(float to, float over, Action callback = null)
        {
            if(activeFade != null) StopCoroutine(activeFade);
            activeFade = StartCoroutine(FadeAsync(to, over, callback));
        }

        private Coroutine activeFade;
        public IEnumerator FadeAsync(float to, float over, Action callback)
        {
            float from = source.volume;
            float inverse = 1f / over;
            for(float elapsed = 0f; elapsed < over; elapsed += Time.deltaTime)
            {
                float fraction = elapsed * inverse;
                float value = Mathf.Lerp(from, to, fraction);
                yield return source.volume = value;
            }
            source.volume = to;
            callback?.Invoke();
        }

        private float remainingSilence;
        private bool hasChosenDuration;
        private void Update()
        {
            if(source.isPlaying)
            {
                hasChosenDuration = false;
                return;
            }

            if(!hasChosenDuration)
            {
                hasChosenDuration = true;
                remainingSilence = UnityEngine.Random.Range(5f, 9f);
            }

            if((remainingSilence -= Time.deltaTime) < 0f) Play();
        }
	}
}
