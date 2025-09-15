using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// Allows a Collectable to float towards the player when they are within a certain range
    /// </summary>
	public class MagneticFollow : MonoBehaviour
	{
        [field: SerializeField] public CircleCollider2D Trigger { get; private set; }
        private float baseRadius;

        [SerializeField] private AnimationCurve pickupCurve;
        [SerializeField] private Transform spriteTransform;

        private float timeFollowing;
        private bool isFollowing;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(isFollowing || !gameObject.activeInHierarchy
            || !other.gameObject.CompareTag("Player")) return;

            isFollowing = true;
            timeFollowing = 0f;
        }

        private float startTime;
        public void Initialise()
        {
            isFollowing = false;
            startTime = Time.time;
            Trigger.radius = baseRadius * PlayerStats.PickupRadius;
        }

        private void Awake() => baseRadius = Trigger.radius;
        private void Update()
        {
            if(isFollowing)
            {
                Vector2 playerPosition = (Vector2) Player.Instance.transform.position + Vector2.up * 0.2f;
                float currentSpeed = 8f * pickupCurve.Evaluate(2f * timeFollowing);
                transform.parent.position = Vector2.Lerp(transform.parent.position, playerPosition, Time.deltaTime * currentSpeed);

                timeFollowing += Time.deltaTime;
            }
            else
            {
                float sineOffset = Mathf.Sin(4f * (Time.time - startTime));
			    spriteTransform.localPosition = new Vector2(0f, 0.2f * sineOffset);
            }
        }
	}
}
