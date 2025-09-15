using Sigmoid.Effects;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class ShufflableCup : MonoBehaviour, IInteractable
    {
        [SerializeField] private CupShuffle puzzle;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private SpriteRenderer shimmer;
        [SerializeField] private Animator animator;
        public bool HasBall { get; set; }

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => puzzle.State == ShuffleState.Inactive || puzzle.State == ShuffleState.Choosing;
        public void HighlightMaterial() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Highlight()
        {
            if(puzzle.State == ShuffleState.Inactive) puzzle.HighlightAll();
            else HighlightMaterial();
        }

        public void UnhighlightMaterial() => sprite.material = MaterialManager.NormalMaterial;
        public void Unhighlight()
        {
            if(puzzle.State == ShuffleState.Inactive) puzzle.UnhighlightAll();
            else UnhighlightMaterial();
        }

        public void InteractWith()
        {
            if(puzzle.State == ShuffleState.Inactive) StartCoroutine(puzzle.Shuffle());
            else StartCoroutine(puzzle.ChooseCup(this));
        }

        public void SetSpeed(float speed) => animator.speed = speed;
        public void SetIndex(int index) => animator.SetInteger("Position", index);
        public void Lift(bool remain = false)
        {
            int index = animator.GetInteger("Position");
            animator.Play($"{(remain ? "Reveal" : "Lift")}_{index}");
        }
    }
}
