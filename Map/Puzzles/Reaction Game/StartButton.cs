using Sigmoid.Players;
using Sigmoid.Effects;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public class StartButton : MonoBehaviour, IInteractable
	{
		[SerializeField] private ReactionGame puzzle;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private SpriteRenderer shimmer;

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => puzzle.State == ReactionState.Inactive;
        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;
        public void InteractWith() => puzzle.StartCoroutine(puzzle.StartGuessing());
	}
}
