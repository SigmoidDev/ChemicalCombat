using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.UI
{
	public class HoverGrow : MonoBehaviour
	{
        [SerializeField] private Vector2 normalSize;
        [SerializeField] private Vector2 hoverSize;
        [SerializeField] private float changeSpeed;

        private void Update()
        {
            if(SceneLoader.Instance.CurrentScene != GameScene.Menu && !PlayerUI.InMenu) return;
            transform.localScale = Vector2.Lerp(transform.localScale, isHovering ? hoverSize : normalSize, changeSpeed * Time.unscaledDeltaTime);
        }

        private bool isHovering;
        public void Hover() => isHovering = true;
        public void Unhover() => isHovering = false;
    }
}
