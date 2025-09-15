using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Effects
{
    /// <summary>
    /// Fades out a sprite based on an AnimationCurve
    /// </summary>
	public class FadeEffect : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer sprite;
		[SerializeField] private AnimationCurve fadeOut;
        [SerializeField] private float baseDuration;
        private float EffectDuration => baseDuration * PlayerStats.EffectDuration;

		private float reciprocalDuration;
		private float timeElapsed;

        private void Awake() => Initialise(transform.position);
		public FadeEffect Initialise(Vector2 position)
		{
			transform.position = position;
			reciprocalDuration = 1f / EffectDuration;
			timeElapsed = 0f;

			return this;
		}

        private void Update()
		{
			if((timeElapsed += Time.deltaTime) >= EffectDuration) return;
			GetAlpha(timeElapsed * reciprocalDuration);
        }

		public void GetAlpha(float fraction)
        {
            float alpha = fadeOut.Evaluate(fraction);

			Color colour = sprite.color;
			colour.a = alpha;
			sprite.color = colour;
        }
	}
}
