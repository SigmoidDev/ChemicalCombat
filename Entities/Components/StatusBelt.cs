using System.Collections.Generic;
using Sigmoid.Chemicals;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Displays all elements applied to a Damageable in an arc above their head
    /// </summary>
	public class StatusBelt : MonoBehaviour
	{
		[SerializeField] private float separation;
		[SerializeField] private float curviness;

		private List<StatusIndicator> statusEffects;
		private void Awake() => statusEffects = new List<StatusIndicator>();

        /// <summary>
        /// Registers a new indicator when a new chemical is inflicted
        /// </summary>
        /// <param name="chemical"></param>
        public void Add(Chemical chemical)
        {
            StatusIndicator status = StatusPooler.Instance.Fetch().Initialise(chemical, transform);
            statusEffects.Add(status);
			ReorderList();
        }

        /// <summary>
        /// Removes the first instance of a chemical in the belt
        /// </summary>
        /// <param name="chemical"></param>
        public void Remove(Chemical chemical)
		{
			foreach(StatusIndicator indicator in statusEffects)
			{
				if(indicator.Chemical == chemical)
				{
					StatusPooler.Instance.Release(indicator);
					statusEffects.Remove(indicator);
					break;
				}
			}
			ReorderList();
		}

        /// <summary>
        /// Removes all indicators in the belt
        /// </summary>
		public void Clear()
		{
            if(statusEffects == null) return;
			foreach(StatusIndicator indicator in statusEffects)
			{
				indicator.transform.SetParent(StatusPooler.Instance.transform);
				StatusPooler.Instance.Release(indicator);
			}
            statusEffects.Clear();
		}

        /// <summary>
        /// Updates the positions of the indicators to follow a slice of a quadratic
        /// </summary>
		private void ReorderList()
		{
			int num = statusEffects.Count;
			for(int i = 0; i < num; i++)
			{
				float x = i - (num - 1) * 0.5f;
				float y = -curviness * x * x;

				statusEffects[i].transform.localPosition = separation * new Vector2(x, y);
			}
		}

        /// <summary>
        /// Updates the alphas of all active indicators
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            foreach(StatusIndicator indicator in statusEffects)
                indicator.Sprite.color = new Color(1f, 1f, 1f, alpha);
        }
	}
}
