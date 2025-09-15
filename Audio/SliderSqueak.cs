using UnityEngine.EventSystems;
using UnityEngine;

namespace Sigmoid.Audio
{
    [RequireComponent(typeof(AudioPlayer))]
	public class SliderSqueak : MonoBehaviour
	{
        [SerializeField] private AudioPlayer player;

		private float squeakTimer;
        public void ResetTimer()
        {
            if(EventSystem.current.currentSelectedGameObject != gameObject) return;
            if(!player.IsPlaying) player.Play();
            squeakTimer = 0.2f;
        }

        private void Update()
        {
            if((squeakTimer -= Time.unscaledDeltaTime) <= 0f) player.Stop();
        }
	}
}
