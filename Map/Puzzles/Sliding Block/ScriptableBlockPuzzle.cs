using UnityEngine;

namespace Sigmoid.Puzzles
{
    [CreateAssetMenu(fileName = "New Block Puzzle", menuName = "Dungeons/Create New Block Puzzle")]
	public class ScriptableBlockPuzzle : ScriptableObject
	{
		public BlockPuzzle puzzle;
	}

    [System.Serializable]
    public class BlockPuzzle
    {
        public bool[] walls;
        public Vector2Int start;
        public Vector2Int exit;

        public BlockPuzzle(bool[] walls, Vector2Int start, Vector2Int exit)
        {
            this.walls = walls;
            this.start = start;
            this.exit = exit;
        }

        /// <summary>
        /// Returns a copy of this puzzle, rotated 90deg clockwise
        /// </summary>
        /// <returns></returns>
        public BlockPuzzle Rotate()
        {
            BlockPuzzle copy = new BlockPuzzle(new bool[49],
                new Vector2Int(start.y, 6 - start.x),
                new Vector2Int(exit.y, 6 - exit.x)
            );

            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    int originalIndex = 7 * y + x;
                    int rotatedIndex = 7 * (6 - x) + y;
                    copy.walls[rotatedIndex] = walls[originalIndex];
                }
            }

            return copy;
        }
    }
}
