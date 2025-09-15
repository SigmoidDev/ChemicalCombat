using System.Collections.Generic;
using Sigmoid.Generation;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class EnemyZone : MonoBehaviour
	{
		[SerializeField] private List<AirlockDoor> doors;
		[SerializeField] private List<PreplacedEnemy> enemies;

        private void Awake() => ResetRoom();

        private bool isUsed;
        private List<Enemy> spawnedEnemies;
        public void ResetRoom()
        {
            if(spawnedEnemies != null)
            {
                foreach(Enemy enemy in spawnedEnemies.ToArray())
                {
                    enemy.OnDeath -= OnDeath;
                    EnemySpawner.Instance.KillEnemy(enemy);
                    spawnedEnemies.Remove(enemy);
                }
            }

            foreach(AirlockDoor door in doors)
                door.Open();

            spawnedEnemies = new List<Enemy>();
            isUsed = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(isUsed || !other.CompareTag("Player")) return;
            isUsed = true;

            foreach(AirlockDoor door in doors) door.Close();
            foreach(PreplacedEnemy placement in enemies)
            {
                Enemy enemy = placement.Spawn();
                spawnedEnemies.Add(enemy);
                enemy.OnDeath += OnDeath;
            }
        }

        private void OnDeath(Enemy enemy)
        {
            enemy.OnDeath -= OnDeath;
            spawnedEnemies.Remove(enemy);

            if(spawnedEnemies.Count == 0)
                foreach(AirlockDoor door in doors)
                    door.Open();
        }
	}
}
