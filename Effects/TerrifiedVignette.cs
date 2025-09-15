using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine.UI;
using UnityEngine;
using Sigmoid.Game;

namespace Sigmoid.Effects
{
	public class TerrifiedVignette : Singleton<TerrifiedVignette>
	{
        private const int LENGTH = 3;

		[SerializeField] private Image[] images = new Image[LENGTH];
        [SerializeField] private Color[] colours = new Color[LENGTH];

        public void UpdateAlphas(int n)
        {
            for(int i = 0; i < LENGTH; i++)
                colours[i].a = Mathf.Clamp01(0.2f * n - i) / PlayerStats.LightLevel;
        }

        private void Update()
        {
            if(SceneLoader.Instance.CurrentScene != GameScene.Labyrinth
            || !FloorManager.Instance.IsIndex(FloorManager.Instance.Floor, 0)) return;

            for(int i = 0; i < LENGTH; i++)
                images[i].color = Color.Lerp(images[i].color, colours[i], Time.deltaTime * 12f);
        }
	}
}
