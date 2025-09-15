using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Mining
{
	public class PanelInteractable : MonoBehaviour, IInteractable
	{
		[SerializeField] private SpriteRenderer panel;

        public bool CanInteract => true;
        public void InteractWith() => DrillingMenu.Instance.Open();
        public void Highlight() => panel.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => panel.material = MaterialManager.NormalMaterial;
    }
}
