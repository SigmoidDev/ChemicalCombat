using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class LaserEmitter : MonoBehaviour
	{
        [Header("Beam")]
        [SerializeField] protected LineRenderer beam;
        [SerializeField] protected BoxCollider2D hitbox;
        [SerializeField] protected Vector2 retractedPosition;
        [SerializeField] protected Vector2 extendedPosition;

        [Space, Header("Timings")]
		[SerializeField] protected float timeOn;
		[SerializeField] protected float timeOff;
        [SerializeField] protected float beamTime;
		[SerializeField] protected float offset;
        protected float elapsed;
        protected bool isOn;

        private void Awake() => elapsed = offset;
        private void Update()
        {
            elapsed += Time.deltaTime;
            if((isOn && elapsed > timeOn) || (!isOn && elapsed > timeOff))
            {
                elapsed -= isOn ? timeOn : timeOff;
                isOn = !isOn;
                hitbox.enabled = isOn;

                if(isOn) beam.SetPosition(0, retractedPosition);
                else beam.SetPosition(1, extendedPosition);
            }

            float lerpFactor = Mathf.InverseLerp(0f, beamTime, elapsed);
            Vector2 lerpedPoint = Vector2.Lerp(retractedPosition, extendedPosition, lerpFactor);

            if(isOn) beam.SetPosition(1, lerpedPoint);
            else beam.SetPosition(0, lerpedPoint);
        }
	}
}
