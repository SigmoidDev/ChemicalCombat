using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class HealthDisplay : Singleton<HealthDisplay>
	{
		[SerializeField] private RectTransform outerRect;
		[SerializeField] private Image potionRect;
		[SerializeField] private Image barRect;

        private void Awake()
        {
            Player.Instance.OnHealthChanged += Refresh;
            PlayerStats.MaxHealth.OnStatChanged += ResizeHealthBar;
            ResizeHealthBar(6);
        }

        /// <summary>
        /// Updates the fillAmounts of the UI components when health updates<br/>
        /// See the Desmos graph for the logic (it's just the ratio of pixel sizes)
        /// </summary>
        /// <param name="fraction"></param>
        public void Refresh(int health)
		{
			potionRect.fillAmount = health / PlayerStats.MaxHealth;
			barRect.fillAmount = health * ratio * 6 - ratio;
		}

        private float ratio;
        public void ResizeHealthBar(int health)
        {
            outerRect.sizeDelta = new Vector2(6 * health + 5, outerRect.sizeDelta.y);
            ratio = 1f / (6f * health - 1f);
        }
	}
}
