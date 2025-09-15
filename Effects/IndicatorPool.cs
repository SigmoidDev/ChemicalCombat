using System.Collections.Generic;
using Sigmoid.Utilities;
using Sigmoid.Enemies;
using Sigmoid.Game;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class IndicatorPool : ObjectPool<DamageIndicator>
	{
        private HashSet<Damageable> handledEnemies;

		private void Start()
        {
            handledEnemies = new HashSet<Damageable>();
            SceneLoader.Instance.OnSceneLoaded += RegisterSpawns; //unsubscribed manually in OnDestroy
        }

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RegisterSpawns;
        }

        private void RegisterSpawns(GameScene scene)
        {
            if(scene == GameScene.Menu) return;
            if(scene != GameScene.Labyrinth)
            {
                foreach(TargetDummy dummy in FindObjectsOfType<TargetDummy>())
                    dummy.Damageable.OnDamage += (damage) => SpawnIndicator(dummy.transform.position, damage); //unsubscribed automatically on unload of Home scene
            }

            EnemySpawner.Instance.OnEnemySpawned += (enemy) => //unsubscribed automatically on unload of current scene (Home or Labyrinth)
            {
                if(handledEnemies.Contains(enemy.Damageable)) return;
                handledEnemies.Add(enemy.Damageable);

                enemy.Damageable.OnDamage += (damage) => SpawnIndicator(enemy.transform.position, damage); //unsubscribed automatically when enemy is kiled
            };
        }

        private void SpawnIndicator(Vector2 position, int damage)
        {
            if(!Options.Current.ShowDamageIndicators) return;

            Vector2 indicatorPosition = position + Vector2.up;
            DamageIndicator indicator = Fetch().Initialise(indicatorPosition, damage);
            indicator.OnComplete += OnComplete; //unsubscribes after being called
        }

        private void OnComplete(DamageIndicator indicator)
        {
            indicator.OnComplete -= OnComplete;
            Release(indicator);
        }
	}
}
