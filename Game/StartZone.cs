using Sigmoid.Generation;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Game
{
	public class StartZone : MonoBehaviour
	{
        private bool used;
		private void OnTriggerEnter2D(Collider2D other)
        {
            if(used || !other.CompareTag("Player")) return;

            used = true;
            (ScriptableFloor nextFloor, ScriptableSize nextSize) = FloorManager.Instance.NextFloor;
            SceneLoader.Instance.EnterLabyrinth(nextFloor, nextSize);
            AudioManager.Instance.FadeOut();
        }
	}
}
