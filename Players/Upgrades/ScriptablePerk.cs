using UnityEngine;

namespace Sigmoid.Upgrading
{
    [CreateAssetMenu(fileName = "New Perk", menuName = "Players/Create New Perk")]
	public class ScriptablePerk : ScriptableObject
	{
		public Perk associatedPerk;
        [Multiline] public string shortDescription;
        [Multiline] public string mediumDescription;
        [Multiline] public string longDescription;
        [Min(1)] public int basePrice;
	}
}
