using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class TutorialExit : MonoBehaviour
	{
		private bool used;
		private void OnTriggerEnter2D(Collider2D other)
        {
            if(used || !other.CompareTag("Player")) return;
            used = true;

            AudioManager.Instance.FadeOut();
            DifficultyManager.CompleteTutorial();
            SceneLoader.Instance.RestartGame();
        }
	}
}
