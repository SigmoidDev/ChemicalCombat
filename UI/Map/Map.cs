using System.Collections.Generic;
using System;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.UI
{
	public class Map : Singleton<Map>
	{
		[SerializeField] private Color[] palette;
		[SerializeField] private Material material;
        public static Material MapMaterial => Instance.material;
        private void Awake() => material = new Material(material);

        [field: SerializeField] public ScriptableArena HomeArea { get; private set; }
        [field: SerializeField] public ScriptableArena Tutorial { get; private set; }
        public float Scale { get; private set; }
		public Vector2 Offset { get; private set; }

        public void Redraw(GameScene scene, List<Room> path, Action<Vector2Int> callback)
        {
            bool[,] map = scene switch
            {
                GameScene.Home => HomeArea.Walls,
                GameScene.Tutorial => Tutorial.Walls,
                _ => MapRenderer.Instance.Dungeon.Map
            };
            Vector2Int size = scene switch
            {
                GameScene.Home => HomeArea.size,
                GameScene.Tutorial => Tutorial.size,
                _ => MapRenderer.Instance.Dungeon.PaddedSize
            };
            Texture2D texture = new Texture2D(size.x, size.y);
            Offset = scene switch
            {
                GameScene.Home => HomeArea.entrance,
                GameScene.Tutorial => Tutorial.entrance,
                _ => Vector2.zero
            };

            bool[,] highlights = new bool[size.x, size.y];
            if(path != null && Perks.Has(Perk.Pathfinder))
            {
                for(int i = 0; i < path.Count; i++)
                {
                    Room room = path[i];
                    int xMin = room.bounds.xMin + room.dungeonReference.Padding.left;
                    int xMax = room.bounds.xMax + room.dungeonReference.Padding.left;
                    int yMin = room.bounds.yMin + room.dungeonReference.Padding.bottom;
                    int yMax = room.bounds.yMax + room.dungeonReference.Padding.bottom;

                    for(int x = xMin; x <= xMax; x++)
                    {
                        for(int y = yMin; y <= yMax; y++)
                        {
                            highlights[x, y] = true;
                        }
                    }

                    if(i >= path.Count - 1) break;
                    Room next = path[i + 1];

                    foreach(Corridor corridor in room.connections)
                    {
                        Room other = corridor.roomA == room ? corridor.roomB : corridor.roomA;
                        if(other != next) continue;

                        foreach(Vector2Int coord in corridor.tiles)
                            highlights[coord.x, coord.y] = true;
                    }
                }
            }

            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    int index = TileMapper.GetColourIndex(x, y, size.x, size.y, map, scene != GameScene.Labyrinth, highlights);
                    if(index == -1) texture.SetPixel(x, y, Color.clear);
                    else texture.SetPixel(x, y, palette[index]);
                }
            }

            texture.filterMode = FilterMode.Point;
			texture.Apply();

            material.SetTexture("_MainTex", texture);
            callback?.Invoke(size);
        }
	}
}
