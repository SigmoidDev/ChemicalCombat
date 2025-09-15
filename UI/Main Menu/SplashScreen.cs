using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class SplashScreen : MonoBehaviour
	{
        private static WaitForSeconds _waitForSeconds0_15 = new WaitForSeconds(0.15f);
        private static WaitForSeconds _waitForSeconds2_5 = new WaitForSeconds(2.5f);
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image sigmoidIcon;
        [SerializeField] private AnimationCurve fadeCurve;
        [SerializeField] private AnimationCurve iconCurve;

		private IEnumerator Start()
        {
            StartCoroutine(FadeIcon());
            yield return _waitForSeconds2_5;

            yield return SceneManager.LoadSceneAsync("Constants", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("Overlays", LoadSceneMode.Additive);

            for(float elapsed = 0f; elapsed <= 0.5f; elapsed += Time.deltaTime)
                yield return canvasGroup.alpha = fadeCurve.Evaluate(elapsed * 2f);

            yield return SceneManager.UnloadSceneAsync("Splash");
        }

        private IEnumerator FadeIcon()
        {
            yield return _waitForSeconds0_15;
            for(float elapsed = 0f; elapsed <= 2f; elapsed += Time.deltaTime)
            {
                Color colour = sigmoidIcon.color;
                colour.a = iconCurve.Evaluate(elapsed * 0.5f);
                sigmoidIcon.color = colour;
                yield return null;
            }
            sigmoidIcon.color = new Color(1f, 1f, 1f, 0f);
        }
	}
}
