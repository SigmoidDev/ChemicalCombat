using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// The IInteractable that allows for the PotionBench to be opened
    /// </summary>
    public class BenchInteractable : MonoBehaviour, IInteractable
    {
		[SerializeField] private SpriteRenderer bench1;
		[SerializeField] private SpriteRenderer bench2;
        [SerializeField] private AudioPlayer player;

        public bool CanInteract => true;
        public void InteractWith()
        {
            PotionBench.Instance.Open();
            player.Play();
        }

        public void Highlight()
		{
			bench1.material = MaterialManager.OutlinedMaterial;
			bench2.material = MaterialManager.OutlinedMaterial;
		}

        public void Unhighlight()
		{
			bench1.material = MaterialManager.NormalMaterial;
			bench2.material = MaterialManager.NormalMaterial;
		}
    }
}
