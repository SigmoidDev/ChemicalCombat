using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// Allows for the instantiation of collectables
    /// </summary>
	public class CollectableSpawner : Singleton<CollectableSpawner>
	{
		[field: SerializeField] public CoinPool CoinPool { get; private set; }
		[field: SerializeField] public HeartPool HeartPool { get; private set; }

        /// <summary>
        /// Drops a certain value in coins, spread across a maximum of 3 by default
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="amount"></param>
		public void DropCoins(Vector2 origin, int amount, int maxCoins = 3)
		{
            amount = (int) (amount * DifficultyManager.CoinsMultipler);
			foreach(int value in CoinManager.GetCoins(amount, maxCoins))
			{
				Coin coin = CoinPool.Fetch().Initialise(origin, value);
                StartCoroutine(LaunchCollectable(coin.Body));
			}
		}

        /// <summary>
        /// Spawns a single heart that wil heal the player on pickup
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="healing"></param>
        public void SpawnHeart(Vector2 origin, int healing)
        {
            Heart heart = HeartPool.Fetch().Initialise(origin, healing);
            StartCoroutine(LaunchCollectable(heart.Body));
        }

        /// <summary>
        /// Enables and disables the gravity on a rigidbody for the launching effect on collectables
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private IEnumerator LaunchCollectable(Rigidbody2D body)
        {
            Vector2 randomForce = new Vector2(Random.Range(-2f, 2f), Random.Range(5f, 6f));
            body.AddForce(randomForce, ForceMode2D.Impulse);

            body.gravityScale = 1.3f;
			float randomTime = Random.Range(0.7f, 0.8f);
			yield return new WaitForSeconds(randomTime);

            if(!body.gameObject.activeInHierarchy) yield break;
			body.gravityScale = 0f;
			body.velocity = Vector2.zero;
        }
	}
}
