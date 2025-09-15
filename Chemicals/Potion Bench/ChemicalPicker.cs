using Sigmoid.Chemicals;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// An element on the shelf that can be drag and dropped
    /// </summary>
	public class ChemicalPicker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		[SerializeField] private Chemical chemical;
		[SerializeField] private Button button;
		[SerializeField] private Image image;
        [SerializeField] private HoverGrow hover;
        [SerializeField] private AudioPlayer player;
        [SerializeField] private ScriptableAudio hoverSound;

        private void Start()
        {
            Refresh();
            SceneLoader.Instance.OnSceneLoaded += RefreshOnLoad; //unsubscribed below in ondestroy
            ChemicalManager.Instance.OnChemicalUnlocked += CheckUpdates; //unsubscribed automatically when the drilling menu is unloaded
        }

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RefreshOnLoad;
        }

        private void RefreshOnLoad(GameScene scene) => Refresh();

        /// <summary>
        /// Checks if this UI element should be refreshed when a chemical is unlocked
        /// </summary>
        /// <param name="chemical"></param>
        private void CheckUpdates(Chemical chemical){ if(chemical == this.chemical) Refresh(); }

        /// <summary>
        /// Fetches the correct sprite from the ChemicalManager if the associated chemical is unlocked
        /// </summary>
        public void Refresh()
		{
			if(!ChemicalManager.IsUnlocked(chemical))
			{
				image.sprite = ChemicalManager.Get(Chemical.None).sprite;
				button.interactable = false;
                gameObject.tag = "Untagged";
				return;
			}

			ScriptableChemical info = ChemicalManager.Get(chemical);
			image.sprite = info.sprite;
			button.interactable = true;
            gameObject.tag = "Clickable";
		}

        /// <summary>
        /// Plays a noise and picks up the chemical only if it is unlocked
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!button.interactable) return;
            PotionBench.Instance.Pickup(chemical);
        }

        /// <summary>
        /// Plays a noise and expands the sprite if the associated chemical is unlocked and nothing is currently picked up
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!button.interactable || PotionBench.Instance.IsDragging) return;
            hover.Hover();
            player.Play(hoverSound, AudioChannel.UI);
        }

        public void OnPointerExit(PointerEventData eventData) => hover.Unhover();
    }
}
