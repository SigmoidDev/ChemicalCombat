using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace Sigmoid.UI
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private Transform holder;
		[SerializeField] private List<Vector3> positions;
		[SerializeField] private List<Light2D> spotlights;

        private int targetIndex;
        public void MoveTo(int index) => targetIndex = index;

        private void Update()
        {
            holder.position = Vector3.Lerp(holder.position, positions[targetIndex], Time.deltaTime * 8f);

            for(int i = 0; i < spotlights.Count; i++)
            {
                Light2D spotlight = spotlights[i];
                spotlight.pointLightInnerRadius = Mathf.Lerp(spotlight.pointLightInnerRadius, i == targetIndex ? 1f : 0f, Time.deltaTime * 12f);
                spotlight.pointLightOuterRadius = Mathf.Lerp(spotlight.pointLightOuterRadius, i == targetIndex ? 2.5f : 0f, Time.deltaTime * 12f);
            }
        }
    }
}
