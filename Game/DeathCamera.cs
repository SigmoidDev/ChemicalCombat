using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Game
{
	public class DeathCamera : Singleton<DeathCamera>
	{
		[SerializeField] private Camera deathCamera;
        private const int WIDTH = 480;
        private const int HEIGHT = 270;

        public Texture2D TakeScreenshot()
        {
            deathCamera.enabled = true;
            deathCamera.transform.position = Player.Instance.transform.position + Vector3.up * 0.5f - Vector3.forward * 10f;
            deathCamera.Render();
            RenderTexture.active = deathCamera.targetTexture;

            Texture2D texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
            texture.filterMode = FilterMode.Point;

            for(int x = 0; x < WIDTH; x++)
            {
                for(int y = 0; y < HEIGHT; y++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    float value = pixel.grayscale;
                    Color grayscale = new Color(value, value, value, 1f);
                    texture.SetPixel(x, y, grayscale);
                }
            }
            texture.Apply();

            RenderTexture.active = null;
            deathCamera.enabled = false;
            return texture;
        }
	}
}
