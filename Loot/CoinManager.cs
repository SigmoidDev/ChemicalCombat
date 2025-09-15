using System.Collections.Generic;
using System;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// Handles everything to do with the player's money
    /// </summary>
	public class CoinManager : Singleton<CoinManager>
	{
		public int Coins { get; private set; }
		public int Earnings { get; private set; }
		public int Expenditure { get; private set; }

        private void Start() => OnCoinsChanged?.Invoke(Coins);

        /// <summary>
        /// Returns whether or not the player can afford a given price (i.e. has >= to that)
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool CanAfford(int amount) => Instance.Coins >= amount;
		public event Action<int> OnCoinsChanged;

        /// <summary>
        /// Adds a certain amount of money to the player's balance
        /// </summary>
        /// <param name="amount"></param>
        public static void Earn(int amount = 1)
        {
            Instance.Earnings += amount;
            Instance.Coins += amount;
			Instance.OnCoinsChanged?.Invoke(Instance.Coins);
        }

        /// <summary>
        /// Spends a given amount of money (check first using CanAfford to avoid negative values)
        /// </summary>
        /// <param name="amount"></param>
        public static void Spend(int amount = 1)
        {
            Instance.Expenditure += amount;
            Instance.Coins -= amount;
			Instance.OnCoinsChanged?.Invoke(Instance.Coins);
        }

        /// <summary>
        /// Evenly divides a number into a list of up to 'max' integers, which defaults to 3
        /// </summary>
        /// <param name="total"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static List<int> GetCoins(int total, int max = 3)
        {
            int larger = Mathf.CeilToInt((float) total / max);
            int smaller = Mathf.FloorToInt((float) total / max);
            int number = total % max;

            List<int> coins = new List<int>();
            for(int i = 0; i < number; i++)
                coins.Add(larger);

            if(smaller == 0) return coins;
            for(int i = 0; i < max - number; i++)
                coins.Add(smaller);
            
            return coins;
        }
    }
}
