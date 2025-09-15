using UnityEngine;
using TMPro;

namespace Sigmoid.Effects
{
	public class DamageIndicator : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI textMeshPro;
		[SerializeField] private Rigidbody2D body;

        [SerializeField] private Gradient damageGradient;

        private const float POSITIONAL_SPREAD = 0.3f;
        private const float ANGULAR_SPREAD = 35f;
        private const float LAUNCH_VELOCITY = 2.5f;

        public DamageIndicator Initialise(Vector2 position, int damage)
        {
            textMeshPro.SetText(damage.ToString());
            textMeshPro.color = damageGradient.Evaluate(damage / 25f % 1f);

            Vector2 randomOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            transform.position = position + POSITIONAL_SPREAD * randomOffset;

            float angle = Random.Range(90f - ANGULAR_SPREAD, 90f + ANGULAR_SPREAD) * Mathf.Deg2Rad;
            Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            body.velocity = Vector2.zero;
            body.totalTorque = 0f;
            body.rotation = 0f;

            body.AddForce(force * LAUNCH_VELOCITY, ForceMode2D.Impulse);
            body.AddTorque(Random.Range(-ANGULAR_SPREAD, ANGULAR_SPREAD));

            lifetime = Random.Range(0.8f, 1.2f);
            reciprocal = 1f / lifetime;
            elapsed = 0f;
            active = true;
            return this;
        }

        public event System.Action<DamageIndicator> OnComplete;

        [SerializeField] private AnimationCurve fadeOut;

        private float lifetime;
        private float reciprocal;
        private float elapsed;
        private bool active;

        private void Update()
        {
            if(!active) return;

            if((elapsed += Time.deltaTime) >= lifetime)
            {
                elapsed = 0f;
                active = false;
                OnComplete?.Invoke(this);
                return;
            }

            textMeshPro.alpha = fadeOut.Evaluate(elapsed * reciprocal);
        }
	}
}
