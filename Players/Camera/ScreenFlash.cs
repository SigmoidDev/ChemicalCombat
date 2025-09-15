using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Sigmoid.Utilities;
using Sigmoid.UI;

namespace Sigmoid.Cameras
{
	public class ScreenFlash : Singleton<ScreenFlash>
	{
        public static readonly Color HurtStartColour = new Color(1.000f, 0.478f, 0.478f, 0.216f);
        public static readonly Color HurtEndColour = new Color(1.000f, 0.443f, 0.443f, 0.000f);

        public static readonly Color FadeStartColour = new Color(0.129f, 0.153f, 0.184f, 0.000f);
        public static readonly Color FadeEndColour = new Color(0.055f, 0.055f, 0.078f, 1.000f);

		[SerializeField] private Image image;
        [SerializeField] private Image circle;
        [SerializeField] private Material material;

		private Coroutine activeFlash;
        public void FlashScreen(float duration, Color startColour, Color endColour)
        {
            if(activeFlash != null) StopCoroutine(activeFlash);
            activeFlash = StartCoroutine(Flash());

            IEnumerator Flash()
            {
                float reciprocalDuration = 1f / duration;
                for(float t = 0f; t < duration; t += Time.deltaTime)
                {
                    float fraction = t * reciprocalDuration;
                    image.color = Color.Lerp(startColour, endColour, fraction);
                    yield return null;
                }

                image.color = endColour;
            }
        }

        private void Awake()
        {
            circle.material = new Material(material);
            circle.material.SetFloat("_Size", 1);
        }

        public void ResetScreen() => image.color = new Color(1f, 1f, 1f, 0f);

		private Coroutine activeCircle;
        public void CircleReveal(float duration, float delay)
        {
            if(activeCircle != null) StopCoroutine(activeCircle);
            activeCircle = StartCoroutine(Circle());

            duration *= Mathf.Min(1f, Options.Current.AnimationTimeMultiplier);
            IEnumerator Circle()
            {
                circle.material.SetFloat("_Size", 0);
                yield return new WaitForSeconds(delay);

                float reciprocalDuration = 1f / duration;
                for(float t = 0f; t < duration; t += Time.deltaTime)
                {
                    float fraction = t * reciprocalDuration;
                    float ease = 1 + (fraction - 1) * (fraction - 1) * (fraction - 1);
                    circle.material.SetFloat("_Size", ease);
                    yield return null;
                }
                circle.material.SetFloat("_Size", 1);
            }
        }
	}
}
