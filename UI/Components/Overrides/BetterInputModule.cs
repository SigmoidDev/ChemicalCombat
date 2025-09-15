using UnityEngine.EventSystems;
using UnityEngine;

namespace Sigmoid.UI
{
	public class BetterInputModule : StandaloneInputModule
	{
        public GameObject HoveredObject => m_PointerData.TryGetValue(StandaloneInputModule.kMouseLeftId, out PointerEventData data) ? data.pointerCurrentRaycast.gameObject : null;
	}
}
