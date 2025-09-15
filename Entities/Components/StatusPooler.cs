using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class StatusPooler : Singleton<StatusPooler>
	{
        [SerializeField] private StatusPool pool;
        public StatusIndicator Fetch() => pool.Fetch();
        public void Release(StatusIndicator thing) => pool.Release(thing);
	}
}
