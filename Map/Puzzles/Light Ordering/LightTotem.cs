using System.Collections;
using Sigmoid.Effects;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public class LightTotem : MonoBehaviour, IInteractable
	{
        [SerializeField] private LightOrdering puzzle;
        [SerializeField] private int myIndex;

		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private SpriteRenderer outlineRenderer;
        [SerializeField] private SpriteRenderer shimmer;
		[SerializeField] private Sprite unlitSprite;
		[SerializeField] private Sprite litSprite;
		[SerializeField] private Sprite crackedSprite;

        public IEnumerator Flash(float duration)
        {
            spriteRenderer.sprite = litSprite;
            yield return new WaitForSeconds(duration);
            spriteRenderer.sprite = unlitSprite;
        }

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => puzzle.State == LightState.Waiting || puzzle.State == LightState.Guessing;
        public void HighlightMaterial() => outlineRenderer.material = MaterialManager.OutlinedMaterial;
        public void Highlight()
        {
            if(puzzle.State == LightState.Waiting) puzzle.HighlightAll();
            else HighlightMaterial();
        }

        public void UnhighlightMaterial() => outlineRenderer.material = MaterialManager.NormalMaterial;
        public void Unhighlight()
        {
            if(puzzle.State == LightState.Waiting) puzzle.UnhighlightAll();
            else UnhighlightMaterial();
        }

        public void InteractWith()
        {
            if(puzzle.State == LightState.Waiting) StartCoroutine(puzzle.ShowSequence());
            else StartCoroutine(puzzle.Guess(myIndex));
        }

        public void Light() => spriteRenderer.sprite = litSprite;
        public void Crack() => spriteRenderer.sprite = crackedSprite;
	}
}
