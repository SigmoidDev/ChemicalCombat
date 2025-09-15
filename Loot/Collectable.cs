using System;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Game
{
    /// <summary>
    /// Represents any hovering item that the player can pick up by walking towards
    /// </summary>
	public abstract class Collectable : MonoBehaviour
	{
        [field: SerializeField] public Rigidbody2D Body { get; private set; }
        [field: SerializeField] public CircleCollider2D Trigger { get; private set; }
        [field: SerializeField] public MagneticFollow Magnet { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
		{
			if(!other.gameObject.CompareTag("Player")) return;
            OnCollect?.Invoke();
            Collect();
		}

        private float baseRadius;
        private void Awake() => baseRadius = Trigger.radius;

        public Action OnCollect;
        protected abstract void Collect();
        protected abstract void Despawn();
        protected void Initialise(Vector2 position)
        {
            transform.position = position;
            Trigger.radius = baseRadius * PlayerStats.PickupRadius;
            Magnet.Initialise();
        }

        private void OnEnable() => SceneLoader.Instance.OnSceneUnloading += DespawnOnSceneUnload;
        private void OnDisable()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneUnloading -= DespawnOnSceneUnload;
        }

        private void DespawnOnSceneUnload(GameScene scene) => Despawn();
    }
}
