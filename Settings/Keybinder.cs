using System.Collections.Generic;
using System.Linq;
using System;
using Sigmoid.Utilities;
using Sigmoid.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows for the mapping of Keys to KeyCodes
    /// </summary>
	public class Keybinder : MonoBehaviour
	{
        [SerializeField] private KeyCode[] bannedKeys;
        private IEnumerable<KeyCode> allowedKeys;

        [SerializeField] private Key associatedKey;
		[SerializeField] private KeyCode defaultKeybind;
		[SerializeField] private UnityEvent<Key, KeyCode> onKeySelected;
        private KeyCode selectedKeybind;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private AudioPlayer audioPlayer;
        [SerializeField] private ScriptableAudio cancelSound;
        [SerializeField] private ScriptableAudio keypressSound;

        /// <summary>
        /// A flag determining whether or not this element should actively search for a keypress
        /// </summary>
        private bool isChoosing;

        /// <summary>
        /// Updates allowedKeys to include all KeyCodes not present in the standard or additional banned keys
        /// </summary>
        private void Awake()
        {
            allowedKeys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>()
            .Where(key => !bannedKeys.Contains(key) && !Keybindings.BANNED_KEYBINDS.Contains(key));

            selectedKeybind = defaultKeybind;
            Deselect();
        }

        /// <summary>
        /// If isChoosing is true, repeatedly loops over all valid keys, checking GetKeyDown on every single one<br/>
        /// (is this really the best way of doing this, who tf knows?)
        /// </summary>
        private void Update()
        {
            if(!isChoosing) return;
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(cancelSound != null)
                audioPlayer.Play(cancelSound, AudioChannel.UI);

                Deselect();
                return;
            }

            foreach(KeyCode keycode in allowedKeys)
            {
                if(Input.GetKeyDown(keycode))
                {
                    Choose(keycode);
                    return;
                }
            }

            //Mouse0 has to be evaluated separately to distinguish it from clicking on a UI element
            bool hoveringOverAnotherElement = UIInput.IsClickable(UIInput.HoveredObject) && UIInput.HoveredObject != gameObject;
            if(Input.GetKeyDown(KeyCode.Mouse0) && !hoveringOverAnotherElement)
                Choose(KeyCode.Mouse0);
        }

        /// <summary>
        /// Chooses a specific key and binds it in settings, before deselecting
        /// </summary>
        /// <param name="keycode"></param>
        public void Choose(KeyCode keycode)
        {
            selectedKeybind = keycode;
            onKeySelected?.Invoke(associatedKey, selectedKeybind);
            Deselect();

            if(keypressSound != null)
            audioPlayer.Play(keypressSound, AudioChannel.UI);
        }

        /// <summary>
        /// Silently updates the selected key and deselects
        /// </summary>
        /// <param name="keycode"></param>
        public void Set(KeyCode keycode)
        {
            selectedKeybind = keycode;
            Deselect();
        }

        /// <summary>
        /// Displays waiting text and begins checking for keypresses
        /// </summary>
        public void Select()
        {
            if(isChoosing) return;

            isChoosing = true;
            button.interactable = false;
            KeybindManager.Instance.Select(this);

            textLabel.SetText("...");
        }

        /// <summary>
        /// Stops detecting keypresses and updates label text to selectedKeybind
        /// </summary>
        public void Deselect()
        {
            isChoosing = false;
            button.interactable = true;
            KeybindManager.Instance.Deselect();

            textLabel.SetText(TextUtilities.AbbreviateKeycode(selectedKeybind));
        }
	}
}
