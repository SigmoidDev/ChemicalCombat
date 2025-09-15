using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.UI
{
    public class ChamberInteractable : MonoBehaviour, IInteractable
    {
		[SerializeField] private SpriteRenderer sprite;
        [SerializeField] private AudioPlayer player;

        public bool CanInteract => true;
        public void InteractWith()
        {
            PerkTree.Instance.Open();
            player.Play();
        }

        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;
    }
}
