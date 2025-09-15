using System.Collections.Generic;
using Sigmoid.Puzzles;
using Sigmoid.Enemies;
using Sigmoid.Audio;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Sigmoid.Generation
{
    [CreateAssetMenu(fileName = "New Floor", menuName = "Dungeons/Create New Floor")]
	public class ScriptableFloor : ScriptableObject
	{
		public TileBase wallTile;
		public TileBase floorTile;
        public ScriptableArena bossArena;
        public List<SpawnGroup> enemyList;
        public List<WeightedFurniture> furnitureList;
		public List<Layout> layoutList;
        public List<PuzzleObject> puzzleObjects;
        public ScriptableAudio musicTrack;

        /// <summary>
        /// Gets a random group of enemies to spawn based on the room's difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static SpawnGroup GetRandomSpawn(Dictionary<SpawnGroup, float> weights, float totalWeight)
        {
            float randomValue = (float) Random.value * totalWeight;
            foreach(KeyValuePair<SpawnGroup, float> weight in weights)
            {
                randomValue -= weight.Value;
                if(randomValue <= 0f) return weight.Key;
            }

            return null;
        }

        private void OnValidate()
        {
            furnitureList.ForEach(item => item.UpdateName());
            layoutList.ForEach(item => item.UpdateName());
        }
	}

    /// <summary>
    /// Represents a possible group of enemies that can be spawned and their respective weightings
    /// </summary>
    [System.Serializable]
	public class SpawnGroup
	{
		public ScriptableEnemy enemy;
		[Min(1)] public int amount;
		[Min(1)] public int cost;

        [Min(0f)] public float baseWeight;
        [Range(0f, 1f)] public float minDifficulty;
        [Range(0f, 1f)] public float maxDifficulty;

        /// <summary>
        /// Gets a desired weight based on a bell-curve formed by the min and max difficulties
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public float GetWeight(float difficulty)
        {
            if(difficulty < minDifficulty || difficulty > maxDifficulty) return 0f;
            float factor = Mathf.InverseLerp(minDifficulty, maxDifficulty, difficulty);
            float bias = 14f * Mathf.Pow(factor * (1f - factor), 2f) + 0.125f;
            return bias * baseWeight;
        }

        /// <summary>
        /// Formats the SpawnGroup as "Number x Name"
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{amount}x {enemy.name}";
    }
}
