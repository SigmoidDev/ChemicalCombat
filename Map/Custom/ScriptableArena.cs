using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Generation
{
    [CreateAssetMenu(fileName = "New Arena", menuName = "Dungeons/Create New Arena")]
	public class ScriptableArena : ScriptableObject
	{
        public Vector2Int entrance;
        public Vector2Int exit;
        public Vector2 centre;
        public Vector2Int size;
		public bool[] walls;
        public List<PrefabPlacement> placements;

        public bool[,] Walls
        {
            get
            {
                bool[,] array2D = new bool[size.x, size.y];
                for(int i = 0; i < walls.Length; i++)
                {
                    int x = i % size.x;
                    int y = Mathf.FloorToInt(i / size.x);
                    array2D[x, y] = walls[i];
                }
                return array2D;
            }
            set
            {
                size.x = value.GetLength(0);
                size.y = value.GetLength(1);
                walls = new bool[size.x * size.y];

                for(int x = 0; x < size.x; x++)
                {
                    for(int y = 0; y < size.y; y++)
                    {
                        int i = size.x * y + x;
                        walls[i] = value[x, y];
                    }
                }
            }
        }
	}

    [System.Serializable]
    public class PrefabPlacement
    {
        public GameObject gameObject;
        public Vector2 position;
    }
}
