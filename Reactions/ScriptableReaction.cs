using UnityEngine;

namespace Sigmoid.Reactions
{
    [CreateAssetMenu(fileName = "New Reaction", menuName = "Chemistry/Create New Reaction")]
	public class ScriptableReaction : ScriptableObject
	{
		[Multiline] public string description;
	}
}
