using UnityEngine;

namespace Sigmoid.Game
{
	public class ElevatorPortal : MonoBehaviour
	{
        private bool used = false;
		private void OnTriggerEnter2D(Collider2D other)
        {
            if(used || !other.CompareTag("Player")) return;
            used = true;

            SceneLoader.Instance.ReturnHome();
        }
	}
}
