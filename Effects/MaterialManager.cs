using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class MaterialManager : Singleton<MaterialManager>
	{
		[SerializeField] private Material normalMaterial;
        public static Material NormalMaterial => Instance.normalMaterial;

		[SerializeField] private Material outlinedMaterial;
        public static Material OutlinedMaterial => Instance.outlinedMaterial;

        [SerializeField] private Material replacementMaterial;
        public static Material ReplacementMaterial => Instance.replacementMaterial;

        [SerializeField] private Material glowingMaterial;
        public static Material GlowingMaterial => Instance.glowingMaterial;
	}
}
