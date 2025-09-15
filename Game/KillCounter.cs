using Sigmoid.Utilities;
using Sigmoid.Enemies;
using System.Collections.Generic;

namespace Sigmoid.Game
{
    /// <summary>
    /// Tracks stats related to the player's kills, including the specific enemy types
    /// </summary>
	public class KillCounter : Singleton<KillCounter>
	{
        private Dictionary<ScriptableEnemy, int> enemyKills;
		public int Kills { get; private set; }
        public void AddKill(Enemy enemy)
        {
            enemy.OnDeath -= AddKill;

            Kills++;
            if(enemyKills.ContainsKey(enemy.EnemyType)) enemyKills[enemy.EnemyType]++;
            else enemyKills[enemy.EnemyType] = 1;
        }

        private void Awake()
        {
            enemyKills = new Dictionary<ScriptableEnemy, int>();
            SceneLoader.Instance.OnSceneLoaded += RegisterSpawns; //unsubscribed manually in OnDestroy below
        }

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RegisterSpawns;
        }

        private void RegisterSpawns(GameScene scene)
        {
            if(scene == GameScene.Menu) return;
            EnemySpawner.Instance.OnEnemySpawned += RegisterEnemy; //unsubscribed automatically on scene unload
        }

        private void RegisterEnemy(Enemy enemy) => enemy.OnDeath += AddKill;
    }
}
