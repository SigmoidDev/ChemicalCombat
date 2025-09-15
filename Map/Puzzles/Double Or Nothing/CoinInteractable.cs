using Sigmoid.Effects;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public class CoinInteractable : MonoBehaviour, IInteractable
	{
		[SerializeField] private DoubleOrNothing puzzle;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private SpriteRenderer shimmer;

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => puzzle.Value != 0 && !puzzle.IsFlipping;
        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;

        public void InteractWith() => StartCoroutine(puzzle.TakeChance());
    }
}
