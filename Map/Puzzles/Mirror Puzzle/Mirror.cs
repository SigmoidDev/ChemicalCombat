using System;
using Sigmoid.Players;
using Sigmoid.Effects;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class Mirror : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private SpriteRenderer shimmer;
        private MirrorPuzzle puzzle;
        public void Initialise(MirrorPuzzle puzzle) => this.puzzle = puzzle;

        /// <summary>
        /// True represents top-left to bottom-right<br/>
        /// False represents top-right to bottom-left
        /// </summary>
        public bool IsFlipped { get; private set; }
        public event Action OnRotate;

        private void Update() => shimmer.enabled = CanInteract;

        public bool CanInteract => !puzzle.IsCompleted;
        public void Highlight() => sprite.material = MaterialManager.OutlinedMaterial;
        public void Unhighlight() => sprite.material = MaterialManager.NormalMaterial;

        public void InteractWith()
        {
            IsFlipped = !IsFlipped;
            OnRotate?.Invoke();
            transform.localScale = new Vector3(IsFlipped ? -1 : 1, 1, 1);
        }
    }
}
