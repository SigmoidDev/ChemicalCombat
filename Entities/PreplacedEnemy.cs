using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// An indicator to spawn an enemy at this position on scene load or manually
    /// </summary>
	public class PreplacedEnemy : MonoBehaviour
	{
		[field: SerializeField] public ScriptableEnemy Type { get; private set; }
        [field: SerializeField] public bool SpawnOnStart { get; private set; } = true;
        [SerializeField] private SpriteRenderer sprite;

        private void Start()
        {
            sprite.enabled = false;
            if(SpawnOnStart)
            {
                Spawn();
                Destroy(gameObject);
            }
        }

        public Enemy Spawn() => EnemySpawner.Instance.SpawnEnemy(Type, transform.position);
    }
}
