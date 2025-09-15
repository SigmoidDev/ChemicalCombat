using System.Collections;
using Sigmoid.Cameras;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class PlayerTeleporter : MonoBehaviour
	{
        private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);
        [SerializeField] private Vector2 destination;
        public event System.Action OnTeleport;

		private bool used = false;
		private void OnTriggerEnter2D(Collider2D other)
        {
            if(used || !other.CompareTag("Player")) return;
            used = true;

            StartCoroutine(TeleportAsync());
        }

        private IEnumerator TeleportAsync()
        {
            MainCamera.FadeOut(0.3f);
            yield return _waitForSeconds0_3;

            Player.Instance.transform.position = destination;
            OnTeleport?.Invoke();

            MainCamera.ResetScreenColour();
            MainCamera.CircleReveal(0.6f, 0.2f);
        }
	}
}
