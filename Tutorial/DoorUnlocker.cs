using Sigmoid.Generation;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class DoorUnlocker : MonoBehaviour
	{
        [SerializeField] private AirlockDoor door;

		private void Awake() => ResetRoom();

		private bool isUsed = false;
        public void ResetRoom()
        {
            isUsed = false;
            door.Close();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(isUsed || !other.CompareTag("Player")) return;
            isUsed = true;

            door.Open();
        }
	}
}
