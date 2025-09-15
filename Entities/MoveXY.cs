using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// A flexible component that moves a transform from A to B, following some easing curve and either stopping, repeating or flipping
    /// </summary>
	public class MoveXY : MonoBehaviour
	{
		[SerializeField] private EndAction action;
        [SerializeField] private bool autoPlay;
        [SerializeField] private float timeOffset;
        [Space]

        [SerializeField] private Vector2 start;
        [SerializeField] private Vector2 end;
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve xCurve;
        [SerializeField] private AnimationCurve yCurve;

        private float playbackRate;
        private float playbackMultiplier;
        private float reciprocalDuration;
        private float elapsed;

        public void UpdateFrequency(float value) => playbackMultiplier = value;

        private void Start(){ if(autoPlay) Play(); }
        private void Update()
        {
            elapsed += Time.deltaTime * playbackRate * playbackMultiplier;
            if(elapsed > duration || elapsed < 0f)
            {
                Complete();
                return;
            }

            float time = elapsed * reciprocalDuration;
            float xLerp = xCurve.Evaluate(time);
            float yLerp = yCurve.Evaluate(time);
            transform.localPosition = new Vector2(
                Mathf.Lerp(start.x, end.x, xLerp),
                Mathf.Lerp(start.y, end.y, yLerp)
            );
        }

        public void Play()
        {
            elapsed = timeOffset;
            reciprocalDuration = 1f / duration;
            playbackRate = 1f;
            playbackMultiplier = 1f;
        }

        private void Complete()
        {
            switch(action)
            {
                case EndAction.Repeat:
                {
                    elapsed = 0f;
                    break;
                }
                case EndAction.PingPong:
                {
                    playbackRate *= -1f;
                    elapsed += Time.deltaTime * playbackRate;
                    break;
                }
            }
        }

        public enum EndAction
        {
            None,
            Repeat,
            PingPong
        }
	}
}
