using System.Collections.Generic;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class EnemySpawner : Singleton<EnemySpawner>
	{
        [field: SerializeField] private EnemyPool Pool { get; set; }

        private List<Enemy> spawnedEnemies;
        public int EnemyCount => spawnedEnemies.Count;
        private void Awake() => spawnedEnemies = new List<Enemy>();

		[SerializeField] private LayerMask groundMask;
        public event System.Action<Enemy> OnEnemySpawned;

        public Enemy SpawnEnemy(ScriptableEnemy type, Room room)
		{
			Vector2 spawnPos = GetSafeSpawn(room);
			return spawnPos == Vector2.zero ? null : SpawnEnemy(type, spawnPos);
		}

        public Enemy SpawnEnemy(ScriptableEnemy type, Vector2 position)
        {
            if(EnemyCount > 99) return null;

            Enemy enemy = Pool.Fetch().Initialise(type, position);
            OnEnemySpawned?.Invoke(enemy);
            spawnedEnemies.Add(enemy);
			return enemy;
        }

        public void KillEnemy(Enemy enemy)
        {
            spawnedEnemies.Remove(enemy);
            Pool.Release(enemy);
        }

		private const int MAX_ATTEMPTS = 30;
		private Vector2 GetSafeSpawn(Room room)
		{
			for(int i = 0; i < MAX_ATTEMPTS; i++)
			{
				Vector2 randomPos = new Vector2(
                    Random.Range(room.interior.xMin, room.interior.xMax),
                    Random.Range(room.interior.yMin, room.interior.yMax)
                );

				bool noWalls = MapRenderer.Instance.Walls.GetTile(Vector3Int.FloorToInt(randomPos)) == null;
				bool noColliders = Physics2D.OverlapCircleAll(randomPos, 1f, groundMask).Length == 0;
				bool offScreen = Vector2.Distance(Player.Instance.transform.position, randomPos) > 8f;

				if(noWalls && noColliders && offScreen) return randomPos;
			}
			return Vector2.zero;
		}
	}
}
