using System.Collections.Generic;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.UI
{
	public class MarkerPool : ObjectPool<Marker>
	{
		[SerializeField] private RoomMarker[] markers;
        private Dictionary<RoomType, Sprite> sprites;
        public Sprite GetSprite(RoomType type) => sprites.TryGetValue(type, out Sprite sprite) ? sprite : null;

        private void Awake()
        {
            sprites = new Dictionary<RoomType, Sprite>();
            foreach(RoomMarker marker in markers)
                sprites.Add(marker.type, marker.sprite);
        }
	}

    [System.Serializable]
    public class RoomMarker
    {
        public RoomType type;
        public Sprite sprite;
    }
}
