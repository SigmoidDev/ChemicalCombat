using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class CoinZone : MonoBehaviour
	{
        private void Awake() => ResetRoom();

		private bool isUsed = false;
        public void ResetRoom() => isUsed = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(isUsed || !other.CompareTag("Player")) return;
            isUsed = true;

            CoinManager.Earn(50 - CoinManager.Instance.Coins);
        }
	}
}
