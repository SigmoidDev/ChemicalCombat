using UnityEngine;

namespace Sigmoid.Chemicals
{
	[CreateAssetMenu(fileName = "New Chemical", menuName = "Chemistry/Create New Chemical")]
	public class ScriptableChemical : ScriptableObject
	{
		public Chemical relatedChemical;
		public Color[] colours;
		public Sprite sprite;
		public Sprite miniSprite;
		public Sprite statusSprite;
		public Sprite digitalSprite;
		public Sprite unlockedSprite;
	}
}
