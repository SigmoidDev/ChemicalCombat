using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Sigmoid.Utilities;

namespace Sigmoid.Secrets
{
	public class InvisibleWall : MonoBehaviour
	{
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TilemapRenderer tilemapRenderer;
        [SerializeField] private Color opaque = Color.white;
        [SerializeField] private Color transparent = Color.white;
        [SerializeField] private float transitionSpeed = 10f;
        [SerializeField] private List<GameObject> hiddenObjects;
        private bool revealed;
        private bool activated;

		private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.gameObject.CompareTag("Player")) return;
            tilemapRenderer.sortingOrder = 2;
            revealed = true;
            activated = true;

            foreach(GameObject hiddenObject in hiddenObjects)
                hiddenObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.gameObject.CompareTag("Player")) return;
            revealed = false;
        }

        private void Update()
        {
            Color targetColour = revealed ? transparent : opaque;
            tilemap.color = Color.Lerp(tilemap.color, targetColour, Time.deltaTime * transitionSpeed);

            if(activated && !revealed && tilemap.color.DistanceTo(targetColour) < 0.01f)
            {
                activated = false;

                tilemapRenderer.sortingOrder = 0;
                foreach(GameObject hiddenObject in hiddenObjects)
                    hiddenObject.SetActive(false);
            }
        }
    }
}
