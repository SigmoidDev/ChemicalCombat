using UnityEngine;

namespace Sigmoid.Reactions
{
	public class ZapLine : MonoBehaviour
	{
		[SerializeField] private LineRenderer line;
		[SerializeField] private float duration;
        private ZapPool pool;

        public void Initialise(ZapPool pool, Vector2 origin, Vector2 target)
        {
            line.positionCount = 2;
            line.SetPosition(0, origin);
            line.SetPosition(1, target);

            this.pool = pool;
            elapsed = 0f;
        }

        private float elapsed;
        private void Update()
        {
            if((elapsed += Time.deltaTime) > duration)
                pool.Release(this);
        }
	}
}
