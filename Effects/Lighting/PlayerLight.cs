using System.Collections.Generic;
using Sigmoid.Players;
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class PlayerLight : MonoBehaviour
	{
        [SerializeField] private Light2D pointLight;
		[SerializeField] private List<ScriptableLight> lights;
        private void Awake() => PlayerStats.LightLevel.OnStatChanged += SwapLight; //unsubscribed automatically with the unload of the Player
        private void SwapLight(int level)
        {
            level = Mathf.Clamp(level, 0, 2);
            ScriptableLight light = lights[level - 1];
            pointLight.pointLightInnerRadius = light.innerRadius;
            pointLight.pointLightOuterRadius = light.outerRadius;
            pointLight.intensity = light.intensity;
            pointLight.falloffIntensity = light.falloff;
        }
    }
}
