using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.UI
{
	public class KeybindManager : Singleton<KeybindManager>
	{
        private Keybinder activeKeybinder;
        public bool IsChoosing => activeKeybinder != null;

        public void Deselect() => activeKeybinder = null;
		public void Select(Keybinder keybinder)
        {
            if(activeKeybinder != null)
                activeKeybinder.Deselect();

            activeKeybinder = keybinder;
        }

        public void Bind(Key key, KeyCode keycode) => Options.Keybinds[key] = keycode;
    }
}
