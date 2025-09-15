using Sigmoid.Utilities;
using Sigmoid.UI;
using UnityEngine;
using TMPro;

namespace Sigmoid.Tutorial
{
	public class KeybindDisplay : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI wKey;
		[SerializeField] private TextMeshProUGUI aKey;
		[SerializeField] private TextMeshProUGUI sKey;
		[SerializeField] private TextMeshProUGUI dKey;
		[SerializeField] private TextMeshProUGUI shiftKey;
		[SerializeField] private TextMeshProUGUI slideKey;
		[SerializeField] private TextMeshProUGUI interactKey;
		[SerializeField] private TextMeshProUGUI potion1Key;
		[SerializeField] private TextMeshProUGUI potion2Key;

        private void Start()
        {
            wKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Up]));
            aKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Left]));
            sKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Down]));
            dKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Right]));
            shiftKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Sprint]));
            slideKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Slide]));
            interactKey.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Interact]));
            potion1Key.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Potion1]));
            potion2Key.SetText(TextUtilities.AbbreviateKeycode(Options.Keybinds[Key.Potion2]));
        }
	}
}
