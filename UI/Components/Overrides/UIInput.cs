using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.UI
{
	public class UIInput : Singleton<UIInput>
	{
		[SerializeField] private BetterInputModule betterInputModule;
        public static GameObject HoveredObject => Instance.betterInputModule.HoveredObject;
        public static bool IsClickable(GameObject obj) => obj != null && obj.CompareTag("Clickable");
	}
}
