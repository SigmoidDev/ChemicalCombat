using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class TutorialCheckpoint : MonoBehaviour
	{
        [field: SerializeField] public Vector2 RespawnPosition { get; private set; }
		public bool IsReached { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(IsReached || !other.CompareTag("Player")) return;
            IsReached = true;
        }
	}
}
