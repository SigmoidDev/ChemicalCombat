using Sigmoid.Effects;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.UI
{
    public class BookInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private AudioPlayer player;

        public bool CanInteract => true;
        public void InteractWith()
        {
            ReactionManual.Instance.Open();
            player.Play();
        }

        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;
    }
}
