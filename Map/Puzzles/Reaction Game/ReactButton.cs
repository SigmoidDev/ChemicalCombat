using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public class ReactButton : MonoBehaviour, IInteractable
	{
		[SerializeField] private ReactionGame puzzle;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer shimmer;
        [SerializeField] private Sprite unlitSprite;
        [SerializeField] private Sprite litSprite;
        [SerializeField] private int myIndex;

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => puzzle.State == ReactionState.Reacting;
        public void Highlight() => spriteRenderer.sprite = litSprite;
        public void Unhighlight() => spriteRenderer.sprite = unlitSprite;
        public void InteractWith() => puzzle.Click(myIndex);
	}
}
