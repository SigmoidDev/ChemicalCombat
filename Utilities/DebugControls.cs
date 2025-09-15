using UnityEngine;
using Sigmoid.UI;
using TMPro;

namespace Sigmoid.Utilities
{
	public class DebugControls : Singleton<DebugControls>
	{
        [SerializeField] private CanvasGroup mainGroup;
        [SerializeField] private CanvasGroup debugGroup;
        [SerializeField] private TextMeshProUGUI debugDisplay;

        private float updateTimer;
        private const float UPDATE_INTERVAL = 0.5f;

        private float framerateTotal;
        private int framerateSamples;

		private void Update()
        {
            if(Input.GetKeyDown(Options.Keybinds[Key.ToggleUI])) mainGroup.alpha = 1f - mainGroup.alpha;
            if(Input.GetKeyDown(Options.Keybinds[Key.Screenshot])) Screenshotter.TakeScreenshot();
            if(Input.GetKeyDown(Options.Keybinds[Key.DebugMenu])) debugGroup.alpha = 1f - debugGroup.alpha;

            if(debugGroup.alpha == 0f) return;

            framerateTotal += 1f / Time.unscaledDeltaTime;
            framerateSamples++;

            if((updateTimer -= Time.deltaTime) > 0f) return;
            updateTimer = UPDATE_INTERVAL;

            debugDisplay.SetText($"FPS: {Mathf.Round(framerateTotal / framerateSamples)}");
            framerateTotal = 0f;
            framerateSamples = 0;
        }
	}
}
