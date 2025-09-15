using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Game
{
	public class Cheats : Singleton<Cheats>
	{
		private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F5))
                CoinManager.Earn(100);
            if(Input.GetKeyDown(KeyCode.F6) && FullscreenMap.IsOpen)
                FullscreenMap.Instance.TeleportMouse();
            if(Input.GetKeyDown(KeyCode.F7))
                Player.Instance.Heal(100);
        }
	}
}
