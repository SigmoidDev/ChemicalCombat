using UnityEngine;

namespace Sigmoid.Enemies
{
	public abstract class TimedParams : AttackerParams
	{
		[field: SerializeField] public float AttackInterval { get; private set; }
		[field: SerializeField] public float AttackDelay { get; private set; }
		[field: SerializeField] public string AttackAnimation { get; private set; }
		[field: SerializeField] public bool OnlyAttackNearby { get; private set; }
	}
}
