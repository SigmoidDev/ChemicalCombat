using System.Collections.Generic;
using System;
using Sigmoid.Cameras;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Players
{
    /// <summary>
    /// Allows the player to right click on any IInteractables
    /// </summary>
	public class Interactor : MonoBehaviour
	{
		[SerializeField] private LayerMask interactionMask;

		private readonly HashSet<Collider2D> highlighted = new HashSet<Collider2D>();
		private readonly HashSet<Collider2D> current = new HashSet<Collider2D>();
        private readonly List<Collider2D> iterator = new List<Collider2D>();
		private readonly Collider2D[] buffer = new Collider2D[30];
		private bool isClickable = false;
		public bool IsHovering => isClickable;

        private void Update()
		{
			if(PlayerUI.InMenu || !Player.Instance.IsAlive) return;

			isClickable = false;
			current.Clear();

			Vector2 mousePosition = MainCamera.MousePosition;// - Vector2.up * 0.25f;
			int numClickable = Physics2D.OverlapCircleNonAlloc(mousePosition, 0.25f, buffer, interactionMask);
			CheckForInteractables(current, numClickable,
                () => Input.GetMouseButtonDown(1) || Input.GetKeyDown(Options.Keybinds[Key.Interact]),
                () => isClickable = true);

            iterator.Clear();
            iterator.AddRange(highlighted);
			foreach(Collider2D collider in iterator)
			{
                if(collider == null)
                {
                    highlighted.Remove(collider);
                    continue;
                }

				if(!current.Contains(collider))
				{
					IInteractable interactable = collider.GetComponent<IInteractable>();
					highlighted.Remove(collider);
					interactable.Unhighlight();
				}
			}
		}

        /// <summary>
        /// Checks for every interactable component in the HashSet, and so long as some condition is met, does something for each one
        /// </summary>
        /// <param name="interactables"></param>
        /// <param name="length"></param>
        /// <param name="condition"></param>
        /// <param name="onFound"></param>
        /// <returns></returns>
		private HashSet<Collider2D> CheckForInteractables(HashSet<Collider2D> interactables, int length, Func<bool> condition, Action onFound = null)
		{
			for(int i = 0; i < length; i++)
			{
				Collider2D collider = buffer[i];
				IInteractable interactable = collider.GetComponent<IInteractable>();
                if(interactable is MonoBehaviour mono && !mono.enabled || !interactable.CanInteract) continue;

				interactables.Add(collider);
				highlighted.Add(collider);
				interactable.Highlight();
				onFound?.Invoke();

				if(condition.Invoke())
				{
					interactable.InteractWith();
					break;
				}
			}
			return interactables;
		}
	}

    /// <summary>
    /// Interface for an object that can be right clicked
    /// </summary>
	public interface IInteractable
	{
        public abstract bool CanInteract { get; }
		public abstract void InteractWith();
		public abstract void Highlight();
		public abstract void Unhighlight();
	}
}
