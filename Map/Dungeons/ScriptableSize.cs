using UnityEngine;

namespace Sigmoid.Generation
{
    [CreateAssetMenu(fileName = "New Size", menuName = "Dungeons/Create New Size")]
	public class ScriptableSize : ScriptableObject
	{
        [Header("Generation")]
		public Vector2Int size;
		public RectOffset padding;
		public int depth;
        [Space]

        [Header("Weightings")]
        public float initial;
        public float gradient;
        public float min;
        public float max;
        public float GetWeight(int n) => Mathf.Clamp(initial + (n - 1) * gradient, min, max);
	}
}
