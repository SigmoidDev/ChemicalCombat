using System.Collections.Generic;
using System.Linq;
using Sigmoid.Generation;
using Sigmoid.Enemies;
using Sigmoid.Game;
using UnityEngine;
using System.Collections;

namespace Sigmoid.Rooms
{
	public class EnemyRoom : PhysicalRoom, IEnumerable<Enemy>
	{
		[SerializeField] private List<Enemy> spawnedEnemies;
        public override PhysicalRoom Initialise(Room room)
        {
            spawnedEnemies = new List<Enemy>();
            return base.Initialise(room);
        }

        protected override void OnEntered()
        {
            LockDoors();
			Queue<ScriptableEnemy> enemies = ChooseEnemies();
			while(enemies.Count > 0) SpawnEnemy(enemies.Dequeue());
        }

        /// <summary>
        /// Chooses what enemies to spawn based on formulae to give a certain number of credits
        /// </summary>
        /// <returns></returns>
        private Queue<ScriptableEnemy> ChooseEnemies()
        {
            float sqrtFloor = Mathf.Sqrt(FloorManager.Instance.FloorNumber);
            float adjustedDifficulty = (Room.difficulty + sqrtFloor - 1f) / sqrtFloor;
            int budget = (int) (10 * Mathf.Pow(2f, Room.difficulty + sqrtFloor) * DifficultyManager.CreditsMultiplier);

            ScriptableFloor currentFloor = FloorManager.Instance.Floor;
            Dictionary<SpawnGroup, float> weights = currentFloor.enemyList.ToDictionary(w => w, w => w.GetWeight(adjustedDifficulty));
            float totalWeight = weights.Values.Sum();

            /*Debug.Log("Difficulty: " + adjustedDifficulty);
            foreach(KeyValuePair<SpawnGroup, float> weight in weights)
                Debug.Log(weight.Key + ": " + weight.Value);*/

            Queue<ScriptableEnemy> enemies = new Queue<ScriptableEnemy>();
            while(budget > 0)
            {
                SpawnGroup group = ScriptableFloor.GetRandomSpawn(weights, totalWeight);
                if(group == null || group.cost > budget) break;

                for(int i = 0; i < group.amount; i++)
                    enemies.Enqueue(group.enemy);

                budget -= group.cost;
            }

            return enemies;
        }

        /// <summary>
        /// Attempts to spawn an enemy at a random point in the room, subscribing them to methods on kill<br/>
        /// Note that this method is not guaranteed to succeed and will throw a warning sometimes
        /// </summary>
        /// <param name="type"></param>
        private void SpawnEnemy(ScriptableEnemy type)
		{
			Enemy enemy = EnemySpawner.Instance.SpawnEnemy(type, Room);
            if(enemy == null)
            {
                Debug.LogWarning("Failed to spawn " + type.name);
                return;
            }

			RegisterEnemy(enemy);
		}

        /// <summary>
        /// Adds an enemy to the spawnedEnemies list and lets it remove itself upon death
        /// </summary>
        /// <param name="enemy"></param>
        public void RegisterEnemy(Enemy enemy)
        {
            //something isn't working, also wizards can tp outside the room
            //Debug.Log("Registered " + enemy.DisplayName, enemy);
            spawnedEnemies.Add(enemy);
			enemy.OnDeath -= OnKill;
			enemy.OnDeath += OnKill;
        }

        /// <summary>
        /// Removes a given enemy from the queue of unkilled enemies on death
        /// </summary>
        /// <param name="enemy"></param>
		private void OnKill(Enemy enemy)
		{
            enemy.OnDeath -= OnKill;
			spawnedEnemies.Remove(enemy);

			if(spawnedEnemies.Count == 0)
            {
                Room.state = RoomState.Cleared;
                RoomGetter.Instance.ClearRoom(this);
                UnlockDoors();
            }
		}

        public IEnumerator<Enemy> GetEnumerator() => spawnedEnemies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => spawnedEnemies.Count;
    }
}
