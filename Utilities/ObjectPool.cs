using System.Collections.Generic;
using System.Collections;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Utilities
{
    /// <summary>
    /// Allows for the recycling of objects when creating and destroying, greatly reducing garbage collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class ObjectPool<T> : MonoBehaviour, IEnumerable<T> where T : MonoBehaviour
	{
		[field: SerializeField] public T Prefab { get; set; }
		[field: SerializeField] public Transform Parent { get; set; }
		private Queue<T> pool = new Queue<T>();
        private void Awake() => SceneLoader.Instance.OnSceneLoaded += ResetPool;
        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= ResetPool;
        }

        private void ResetPool(GameScene scene) => pool = new Queue<T>();

        /// <summary>
        /// Fetches an object, which should then be initialised<br/>
        /// Equivalent to Instantiate in normal terms
        /// </summary>
        /// <returns></returns>
        public T Fetch()
		{
			if(pool.Count == 0) return Instantiate(Prefab, Parent);

			T thing = pool.Dequeue();
			thing.gameObject.SetActive(true);
			return thing;
		}

        /// <summary>
        /// Hides an object and places it back in the pool, rendering it inactive<br/>
        /// Equivalent to Destroy in normal terms
        /// </summary>
        /// <param name="thing"></param>
		public void Release(T thing)
		{
			thing.gameObject.SetActive(false);
			pool.Enqueue(thing);
		}

        public IEnumerator<T> GetEnumerator() => pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => pool.Count;
    }
}
