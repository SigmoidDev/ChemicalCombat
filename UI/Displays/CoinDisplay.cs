using Sigmoid.Game;
using UnityEngine;
using TMPro;

namespace Sigmoid.UI
{
    /// <summary>
    /// Updates a TextMeshPro text component to display the current number of coins
    /// </summary>
	public class CoinDisplay : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI counter;

        private void Awake()
        {
            CoinManager.Instance.OnCoinsChanged += RefreshText; //unsubscribed manually in OnDestroy
            RefreshText(CoinManager.Instance.Coins);
        }
        private void OnDestroy()
        {
            if(!CoinManager.InstanceExists) return;
            CoinManager.Instance.OnCoinsChanged += RefreshText;
        }

        private void RefreshText(int newAmount) => counter.SetText(newAmount.ToString());
	}
}
