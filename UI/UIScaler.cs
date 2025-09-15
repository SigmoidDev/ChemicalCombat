using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
    [RequireComponent(typeof(CanvasScaler))]
	public class UIScaler : MonoBehaviour
	{
		[SerializeField] private CanvasScaler canvasScaler;
		[SerializeField] private Vector2 referenceResolution;

        private void Awake()
        {
            Resize(Options.Current.UIScale);
            OptionsMenu.Instance.OnUIResized += Resize;
        }

        /// <summary>
        /// Resizes the canvas based on an exponential curve going from 0.67x -> 1.5x, with 50% returning 1.0x
        /// </summary>
        /// <param name="newScale"></param>
        private void Resize(float newScale) => canvasScaler.referenceResolution = referenceResolution * Mathf.Pow(1.5f, 1f - 0.02f * newScale);
    }
}
