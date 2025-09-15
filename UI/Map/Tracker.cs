using Sigmoid.Enemies;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class Tracker : MonoBehaviour
	{
        [SerializeField] private RectTransform rect;
		[SerializeField] private Image image;

        private TrackerPool pool;
        private Enemy target;

        public Tracker Initialise(TrackerPool pool, Enemy target)
        {
            this.pool = pool;
            this.target = target;

            target.OnDeath -= TargetKilled;
            target.OnDeath += TargetKilled; //unsubscribes itself on call

            return this;
        }

        private void TargetKilled(Enemy enemy)
        {
            target = null;
            enemy.OnDeath -= TargetKilled;
            pool.Release(this);
        }

        private void Update()
        {
            if(target == null) return;
            rect.anchoredPosition = target.transform.position;
        }
	}
}
