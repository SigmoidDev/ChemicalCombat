using UnityEngine;

namespace Sigmoid.UI
{
	public class TreeQuadrant : MonoBehaviour
	{
		[field: SerializeField] public Color[] HiddenColours { get; private set; }
        [field: SerializeField] public Color[] AvailableColours { get; private set; }
        [field: SerializeField] public Color[] PurchasedColours { get; private set; }
        [field: SerializeField] public bool OnRightSide { get; private set; }
	}
}
