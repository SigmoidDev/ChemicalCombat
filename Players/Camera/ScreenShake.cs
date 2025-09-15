using System.Collections;
using Sigmoid.UI;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Cameras
{
	public class ScreenShake : Singleton<ScreenShake>
	{
        [SerializeField] private Transform shaker;

		private Coroutine activeShake;
        public void ShakeScreen(float duration, float magnitude, float damping, bool unscaled = false)
        {
            if(Options.Current.ScreenShakeStrength == 0f) return;

            if(activeShake != null) StopCoroutine(activeShake);
            activeShake = StartCoroutine(Shake());

            IEnumerator Shake()
            {
                float strength = magnitude * 0.01f * Options.Current.ScreenShakeStrength;

                //yo wait i never realised you could do this until now
                for(float t = 0f; t < duration; t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime)
                {
                    float x = Random.Range(-strength, strength) * 0.5f;
                    float y = Random.Range(-strength, strength) * 0.5f;
                    shaker.localPosition = new Vector2(x, y);

                    strength *= damping;
                    yield return null;
                }

                shaker.localPosition = Vector2.zero;
            }
        }
	}
}
