using Sigmoid.Effects;
using Sigmoid.Players;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Interactables
{
	public class ChestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private LootChest chest;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private SpriteRenderer shimmer;

        private void Update() => shimmer.enabled = CanInteract;

        private bool alreadyUsed;
        public bool CanInteract => !alreadyUsed && !chest.IsLocked;

        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;

        public void InteractWith()
        {
            chest.Open();
            alreadyUsed = true;

            //i know this was supposed to be event based, but i can't be bothered making an event for every time a chest is instantiated
            PersistentStats.OpenChest();
        }
    }
}
