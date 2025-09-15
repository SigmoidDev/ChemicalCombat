using UnityEngine;
using DG.Tweening;

namespace Sigmoid.Tutorial
{
	public class CyclingPages : MonoBehaviour
	{
		[SerializeField] private CanvasGroup[] groups;
        [SerializeField] private float timeOnEach;

        private float elapsed;
        private int index;

        private void Update()
        {
            if((elapsed += Time.deltaTime) > timeOnEach)
            {
                elapsed -= timeOnEach;
                groups[index].DOFade(0f, 0.3f);
                groups[index = (index + 1) % groups.Length].DOFade(1f, 0.3f);
            }
        }
	}
}
