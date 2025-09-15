using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.UI
{
	public class CursorManager : Singleton<CursorManager>
	{
		[SerializeField] private Texture2D normalCursor;
		[SerializeField] private Texture2D crosshairCursor;
		[SerializeField] private Texture2D clickCursor;

		private void Update()
		{
            //No need to update the cursor if not tabbed in
            if(!Application.isFocused) return;

            //If switching scene, just use a normal cursor (avoids NullReferenceExceptions)
            if(SceneLoader.Instance.IsLoading){ SetCursor(CursorType.Normal); return; }

            //If in a menu, use click cursor if hovering over an object tagged with Clickable; otherwise normal cursor
            bool isInAnyMenu = SceneLoader.Instance.CurrentScene == GameScene.Menu || PlayerUI.InMenu || !Player.Instance.IsAlive;
			if(isInAnyMenu){ SetCursor(UIInput.IsClickable(UIInput.HoveredObject) ? CursorType.Click : CursorType.Normal); return; }

            //If in game and hovering over an interactable object, use click cursor; otherwise crosshair
            SetCursor(Player.Instance.Interactor.IsHovering ? CursorType.Click : CursorType.Crosshair);
		}

        private CursorType previousCursor;
		public void SetCursor(CursorType newCursor)
		{
			if(newCursor == Instance.previousCursor){ return; }
            Instance.previousCursor = newCursor;

            switch(newCursor)
            {
				case CursorType.Normal:
					Cursor.SetCursor(Instance.normalCursor, new Vector2(16, 16), CursorMode.Auto);
					break;
				case CursorType.Crosshair:
					Cursor.SetCursor(Instance.crosshairCursor, new Vector2(64, 64), CursorMode.Auto);
					break;
				case CursorType.Click:
					Cursor.SetCursor(Instance.clickCursor, new Vector2(40, 16), CursorMode.Auto);
					break;
			}
		}
	}

	public enum CursorType
	{
		Normal,
		Crosshair,
		Click
	}
}
