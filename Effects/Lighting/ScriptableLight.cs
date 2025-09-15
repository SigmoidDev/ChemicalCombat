using UnityEngine;

namespace Sigmoid.Effects
{
    [CreateAssetMenu(fileName = "New Light", menuName = "Players/Create New Light")]
	public class ScriptableLight : ScriptableObject
	{
		[Min(0)] public float innerRadius;
		[Min(0)] public float outerRadius;
		[Range(0f, 1f)] public float intensity;
		[Range(0f, 1f)] public float falloff;
	}
}
