using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Hitbox", menuName = "Enemies/Create New Hitbox")]
	public class ScriptableHitbox : ScriptableObject
	{
        public Vector2 colliderCentre;
        public Vector2 colliderSize;
        public Vector2 pathfinderSize;
        public Vector2 hitboxCentre;
        public Vector2 hitboxSize;
	}
}
