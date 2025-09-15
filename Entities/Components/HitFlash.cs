using System.Collections;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class HitFlash : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer sprite;
        private Coroutine activeFlash;

		public void Flash(FlashData data = null)
		{
            data ??= FlashData.Default;

            if(activeFlash != null) StopCoroutine(activeFlash);
            activeFlash = StartCoroutine(CFlash());

            IEnumerator CFlash()
            {
                for(float elapsed = 0; elapsed <= data.duration; elapsed += Time.deltaTime)
                {
				    sprite.color = Color.Lerp(data.colour, Color.white, Time.deltaTime * data.reciprocal);
				    yield return null;
                }
			    sprite.color = Color.white;
            }
		}
	}

    public class FlashData
    {
        public readonly Color colour;
        public readonly float duration;
        public readonly float reciprocal;

        public FlashData(Color colour, float duration)
        {
            this.colour = colour;
            this.duration = duration;
            reciprocal = 1f / duration;
        }

        public static readonly FlashData Default = new FlashData(
            new Color(1.000f, 0.482f, 0.482f, 1.000f),
            0.12f
        );
    }
}
