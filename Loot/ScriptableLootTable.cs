using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Interactables
{
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "Players/Create New Loot Table")]
	public class ScriptableLootTable : ScriptableObject
	{
		[SerializeField] private Vector2Int coins;
        public int Coins => Random.Range(coins.x, coins.y + 1);

        public List<Vector2Int> hearts;
	}
}
