using System.Collections;
using Sigmoid.Upgrading;
using Sigmoid.Audio;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Players
{
    /// <summary>
    /// A component that handles player movement
    /// </summary>
	public class Movement : MonoBehaviour
	{
        [field: SerializeField] public Rigidbody2D Body { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [SerializeField] private AudioPlayer slidePlayer;

        public Vector2 ExternalVelocity { get; set; }
        private const float DRAG_COEFFICIENT = 100f;

        /// <summary>
        /// Gets the player movement and checks for sliding
        /// </summary>
        private void Update()
        {
            if(PlayerUI.InMenu || !Player.Instance.IsAlive) return;
            ExternalVelocity = Vector2.Lerp(ExternalVelocity, Vector2.zero, DRAG_COEFFICIENT * Time.deltaTime);

            Vector2 direction = Options.Keybinds.GetMovement().normalized;
            if(IsSliding) return;

            bool isSprinting = Input.GetKey(Options.Keybinds[Key.Sprint]);
            float speed = PlayerStats.MoveSpeed * 0.05f;
            if(isSprinting) speed *= 1.6f;

            Body.velocity = direction * speed + ExternalVelocity;

            slideCooldown -= Time.deltaTime;
            if(slideCooldown <= 0f && Input.GetKeyDown(Options.Keybinds[Key.Slide]))
                StartCoroutine(Slide(direction, isSprinting));
        }

		[SerializeField] private AnimationCurve shortCurve;
		[SerializeField] private AnimationCurve longCurve;
        public bool IsSliding { get; private set; }
        private float slideCooldown;

        /// <summary>
        /// Performs a variable-duration slide depending on whether the slide key was tapped or held (>0.2s)
        /// </summary>
        /// <param name="slideDirection"></param>
        /// <returns></returns>
        private IEnumerator Slide(Vector2 slideDirection, bool wasSprinting)
        {
            IsSliding = true;
            slideCooldown = Perks.Has(Perk.Athletic) ? 0.5f : 0.75f;
            slidePlayer.Play();

            Animator.speed = 1f;
            Animator.Play("Slide");

            AnimationCurve selectedCurve = shortCurve;
            bool isCharging = Perks.Has(Perk.Athletic);
            float timeHeld = 0f;
            float slideDuration = 0.5f;
            float elapsed = 0f;

            while(elapsed <= slideDuration)
            {
                if(Input.GetKeyUp(Options.Keybinds[Key.Slide]))
                    isCharging = false;

                if(isCharging && (timeHeld += Time.deltaTime) > 0.2f)
                {
                    selectedCurve = longCurve;
                    slideDuration = 0.75f;
                    slideCooldown += 0.15f;
                    isCharging = false;
                    StartCoroutine(PauseAnimation(0.2f, 0.1f));
                }

                float speed = PlayerStats.MoveSpeed * 0.1f * selectedCurve.Evaluate(elapsed);
                if(wasSprinting) speed *= 1.5f;
                Body.velocity = slideDirection * speed + ExternalVelocity;

                yield return elapsed += Time.deltaTime;
            }

            IsSliding = false;
        }

        /// <summary>
        /// Temporarily sets the speed of the Animator to 0
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator PauseAnimation(float duration, float delay)
        {
            yield return new WaitForSeconds(delay);
            Animator.speed = 0f;
            yield return new WaitForSeconds(duration);
            Animator.speed = 1f;
        }
	}
}
