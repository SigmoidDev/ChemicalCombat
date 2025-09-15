using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace Sigmoid.Cameras
{
    /// <summary>
    /// Centralised controls for the player camera and related effects
    /// </summary>
	public class MainCamera : Singleton<MainCamera>
	{
        private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);
        [SerializeField] private Transform target;
		public Transform Target
        {
            get
            {
                if(target == null) target = Player.Instance.transform;
                return target;
            }
            set => target = value;
        }

		[field: SerializeField] public Camera Camera { get; private set; }
		[field: SerializeField] public UniversalAdditionalCameraData AdditionalData { get; private set; }

        /// <summary>
        /// Gets the position of the camera in world space
        /// </summary>
        public static Vector2 CameraPosition => Instance.Camera.transform.position;

        /// <summary>
        /// Gets the current mouse position in world space
        /// </summary>
        public static Vector2 MousePosition => Instance.Camera.ScreenToWorldPoint(Input.mousePosition);

        /// <summary>
        /// Determines whether or not a given Vector2 lies within the padded bounds of the camera
        /// </summary>
        /// <param name="position"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static bool WithinView(Vector2 position, float padding)
        {
            Vector2 cameraPosition = Instance.Camera.transform.position;
            float viewHeight = Instance.Camera.orthographicSize;
            float viewWidth = Instance.Camera.aspect * viewHeight;

            float minX = cameraPosition.x - viewWidth - padding;
            float maxX = cameraPosition.x + viewWidth + padding;
            float minY = cameraPosition.y - viewWidth - padding;
            float maxY = cameraPosition.y + viewWidth + padding;

            return position.x >= minX && position.x <= maxX
                && position.y >= minY && position.y <= maxY;
        }


        /// <summary>
        /// Shakes the camera for some number of seconds with the parameters provided
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="magnitude"></param>
        /// <param name="damping"></param>
        public static void ShakeScreen(float duration, float magnitude, float damping, bool unscaled = false) => ScreenShake.Instance.ShakeScreen(duration, magnitude, damping, unscaled);

        /// <summary>
        /// Flashes the screen to the startColour and then fades to endColour
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="startColour"></param>
        /// <param name="endColour"></param>
        public static void FlashScreen(float duration, Color startColour, Color endColour) => ScreenFlash.Instance.FlashScreen(duration, startColour, endColour);

        /// <summary>
        /// Fades the screen to black over the duration provided
        /// </summary>
        /// <param name="duration"></param>
        public static void FadeOut(float duration) => ScreenFlash.Instance.FlashScreen(duration * Mathf.Min(1f, Options.Current.AnimationTimeMultiplier), ScreenFlash.FadeStartColour, ScreenFlash.FadeEndColour);

        /// <summary>
        /// Resets the screen back to clear
        /// </summary>
        public static void ResetScreenColour() => ScreenFlash.Instance.ResetScreen();

        /// <summary>
        /// Fades the screen in from a circle going outwards from the center
        /// </summary>
        public static void CircleReveal(float duration, float delay) => ScreenFlash.Instance.CircleReveal(duration, delay);



        [SerializeField] private float lerpSpeed = 10f;
        private void LateUpdate() => transform.position = Vector2.Lerp(transform.position, Target.position, Time.deltaTime * lerpSpeed);



        /// <summary>
        /// Subscribes to changes to TogglePostProcessing
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            AdditionalData.renderPostProcessing = Options.Current.TogglePostProcessing;
            while(!OptionsMenu.InstanceExists) yield return _waitForSeconds0_1; //waits for the scene to fully load
            OptionsMenu.Instance.OnPostProcessingToggled += (toggled) => AdditionalData.renderPostProcessing = toggled; //unsubscribed when Interface is unloaded
        }
    }
}
