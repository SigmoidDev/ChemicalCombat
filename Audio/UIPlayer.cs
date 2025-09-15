using UnityEngine.EventSystems;
using UnityEngine;

namespace Sigmoid.Audio
{
    [RequireComponent(typeof(AudioPlayer))]
	public class UIPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
	{
		[SerializeField] private AudioPlayer player;
        [SerializeField] private ScriptableAudio hoverSound;
        [SerializeField] private ScriptableAudio clickSound;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSound != null)
            player.Play(hoverSound, AudioChannel.UI);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickSound != null)
            player.Play(clickSound, AudioChannel.UI);
        }
    }
}
