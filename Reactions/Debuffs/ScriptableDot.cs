using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Buffs
{
	[CreateAssetMenu(fileName = "New DoT", menuName = "Enemies/Create New DoT")]
	public class ScriptableDot : ScriptableObject
	{
		public DotType relatedDot;
		public int damage;
        public DamageType type;
		public float interval;
		public Color colour;
		public GameObject particles;
	}
}
