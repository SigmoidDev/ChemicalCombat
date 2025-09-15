using UnityEngine;

namespace Sigmoid.Game
{
	public class ElevatorDoor : MonoBehaviour
	{
        [SerializeField] private Animator animator;
        private void Awake() => animator.Play("Close");

		private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("Player")) return;
            animator.Play("Open");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.CompareTag("Player")) return;
            if(!gameObject.activeInHierarchy) return;
            animator.Play("Close");
        }
	}
}
