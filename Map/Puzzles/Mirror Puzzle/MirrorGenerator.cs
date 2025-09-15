using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public static class MirrorGenerator
	{
        private const int MAX_ATTEMPTS = 40;
        private const int MIN_DISTANCE = 2;

        /// <summary>
        /// Returns the positions of all mirrors within the generated 11x7 grid
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
		public static (List<Vector2Int>, Vector2Int) GenerateMirrorPuzzle(Random rng, int minMirrors)
        {
            Vector2Int direction = Vector2Int.right;
            Vector2Int position = new Vector2Int(0, 3);
            HashSet<Vector2Int> path = new HashSet<Vector2Int>(){ position };
            List<Vector2Int> mirrors = new List<Vector2Int>();

            int attempts = 0; //prevents the game from crashing
            while(mirrors.Count < minMirrors && attempts++ < MAX_ATTEMPTS)
            {
                int upperLengthBound =
                direction == Vector2Int.up ? 7 - position.y :
                direction == Vector2Int.down ? 1 + position.y :
                direction == Vector2Int.right ? 11 - position.x :
                direction == Vector2Int.left ? 1 + position.x : 0;
                bool isHorizontal = direction.y == 0;

                if(upperLengthBound < MIN_DISTANCE)
                {
                    direction *= -1;
                    continue;
                }

                int chosenLength = -1;
                IEnumerable<int> possibleLengths = Enumerable.Range(MIN_DISTANCE, upperLengthBound - MIN_DISTANCE).OrderBy(n => rng.NextDouble());
                foreach(int possibleLength in possibleLengths)
                {
                    Vector2Int targetPosition = position + possibleLength * direction;
                    bool isValid = true;

                    foreach(Vector2Int existingMirror in mirrors)
                    {
                        if(isHorizontal && existingMirror.x == targetPosition.x
                        || !isHorizontal && existingMirror.y == targetPosition.y)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if(isValid)
                    {
                        chosenLength = possibleLength;
                        break;
                    }
                }

                //since i can't be bothered fixing the algorithm, just try generating them until it works
                if(chosenLength == -1)
                {
                    direction *= -1;
                    continue;
                }

                for(int i = 0; i < chosenLength; i++)
                {
                    position += direction;
                    path.Add(position);
                }
                mirrors.Add(position);

                int flip = rng.NextDouble() <= 0.5 ? -1 : 1;
                direction = flip * new Vector2Int(direction.y, direction.x);
            }

            //if it failed to generate within the max number of attempts
            if(attempts >= MAX_ATTEMPTS) return (null, default);

            while(position.x < 11)
            {
                position += Vector2Int.right;
                path.Add(position);
            }

            /*ADDS RANDOM OTHER MIRRORS
            for(int x = 0; x < 11; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    Vector2Int randomPosition = new Vector2Int(x, y);
                    if(rng.NextDouble() < 0.05 && !path.Contains(randomPosition))
                        mirrors.Add(randomPosition);
                }
            }*/

            return (mirrors, position);
        }
	}
}
