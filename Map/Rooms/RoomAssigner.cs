using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Puzzles;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Rooms
{
    /// <summary>
    /// Class for assigning types to each room in the dungeon based on its position along or outside the path
    /// </summary>
	public class RoomAssigner : Singleton<RoomAssigner>
	{
        [SerializeField] private EnemyRoom enemyRoom;
        [SerializeField] private LootRoom lootRoom;
        [SerializeField] private PuzzleRoom puzzleRoom;
        [SerializeField] private BossRoom bossRoom;
        [SerializeField] private PresetRoom presetRoom;

        /// <summary>
        /// Instantiates the correct type of prefab based on the room's type
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public PhysicalRoom GenerateRoom(Vector2 position, RoomType type)
        {
            if(type == RoomType.Enemy) return Instantiate(enemyRoom, position, Quaternion.identity, transform);
            if(type == RoomType.Loot) return Instantiate(puzzleRoom, position, Quaternion.identity, transform); //TEMP
            if(type == RoomType.Puzzle) return Instantiate(puzzleRoom, position, Quaternion.identity, transform);
            if(type == RoomType.Boss) return Instantiate(bossRoom, position, Quaternion.identity, transform);
            return Instantiate(presetRoom, position, Quaternion.identity, transform);
        }

        //Distribute in a 1:2 ratio (i haven't done loot rooms yet :yikes:)
        public const float LOOT_FACTOR = 0f;
        public const float PUZZLE_FACTOR = 2f;
        public const float SPECIAL_RATIO = LOOT_FACTOR / (LOOT_FACTOR + PUZZLE_FACTOR);

        //How much random noise is applied to the percentages
        public const float VARIATION = 0.1f;

        /// <summary>
        /// Assigns the designated room types such that:<br/>
        /// - Along the path, rooms are all of type enemy, with a set number of puzzle and loot rooms located evenly throughout<br/>
        /// - Everywhere else the rooms are mostly enemy, with the chance of being special becoming higher further away from the path
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static Dungeon AssignTypes(Dungeon dungeon, float seed)
        {
            Random rng = new Random((int) (seed * 100));
            AssignAlongPath(dungeon, rng);

            Dictionary<Room, float> difficulties = new Dictionary<Room, float>();
            foreach(Room room in dungeon.Rooms)
            {
                if(room.type != RoomType.Unassigned) continue;
                if(room.connections.Count == 0)
                {
                    Debug.LogError("Disconnected Room!", room.physicalRoom);
                    continue;
                }

                float assignedDifficulty = GetDifficultyByConnections(room, dungeon.Path);
                difficulties.Add(room, assignedDifficulty);
                AssignOtherRoomType(room, rng);
            }

            AssignPuzzles(dungeon, rng);
            BlurRoomDifficulties(difficulties);
            return dungeon;
        }



        /// <summary>
        /// Randomly assigns puzzle types to each of the puzzle rooms, ensuring an even distribution
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="rng"></param>
        private static void AssignPuzzles(Dungeon dungeon, Random rng)
        {
            List<PuzzleType> allPuzzles = new List<PuzzleType>
            {
                PuzzleType.TicTacToe,
                PuzzleType.SlidingBlock,
                PuzzleType.LightOrdering,
                PuzzleType.DoubleOrNothing,
                PuzzleType.ReactionGame,
                PuzzleType.MirrorPuzzle,
                PuzzleType.CupShuffle
            };

            List<PuzzleType> unassignedPuzzles = allPuzzles.OrderBy(p => rng.NextDouble()).ToList();
            foreach(Room room in dungeon.Rooms)
            {
                if(room.type != RoomType.Puzzle) continue;

                //if all puzzles have been used up, add a new random set
                if(unassignedPuzzles.Count == 0) unassignedPuzzles = allPuzzles.OrderBy(p => rng.NextDouble()).ToList();

                //pop and assign the last element of the randomly sorted list
                room.puzzleType = unassignedPuzzles[^1];
                unassignedPuzzles.RemoveAt(unassignedPuzzles.Count - 1);
            }
        }

        /// <summary>
        /// Takes all rooms and averages the difficulty of their neighbours
        /// </summary>
        /// <param name="difficulties"></param>
        private static void BlurRoomDifficulties(Dictionary<Room, float> difficulties)
        {
            //actually sets the difficulties based on the dictionary
            foreach(KeyValuePair<Room, float> difficultyPair in difficulties)
                difficultyPair.Key.difficulty = difficultyPair.Value;

            //takes a non-weighted average of all neighbours
            Dictionary<Room, float> difficultiesBlurred = new Dictionary<Room, float>();
            foreach(KeyValuePair<Room, float> difficultyPair in difficulties)
            {
                float difficultySum = 0f;
                foreach(Corridor corridor in difficultyPair.Key.connections)
                {
                    Room other = corridor.roomA == difficultyPair.Key ? corridor.roomB : corridor.roomA;
                    difficultySum += other.difficulty;
                }
                difficultiesBlurred.Add(difficultyPair.Key, difficultySum / difficultyPair.Key.connections.Count);
            }

            //reassigns the blurred difficulties
            foreach(KeyValuePair<Room, float> difficultyPair in difficultiesBlurred)
                difficultyPair.Key.difficulty = difficultyPair.Value;
        }

        /// <summary>
        /// Chooses the type of room for any unassigned room not on the path<br/>
        /// Probabilities are linearly interpolated based on pathAffinity such that further away rooms have a higher special chance<br/>
        /// Special rooms are divided randomly into either Loot or Puzzle based on SPECIAL_RATIO
        /// </summary>
        /// <param name="room"></param>
        /// <param name="rng"></param>
        private static void AssignOtherRoomType(Room room, Random rng)
        {
            float enemyChance = Mathf.Lerp(MinEnemyRoomChance, MaxEnemyRoomChance, room.pathAffinity);
            if(rng.NextDouble() < enemyChance) room.type = RoomType.Enemy;
            else
            {
                Vector2Int size = room.bounds.size;
                room.type = size.x > 8 && size.y > 8 ? RoomType.Puzzle : RoomType.Enemy;
                //room.type = RoomType.Puzzle; //rng.NextDouble() < SPECIAL_RATIO ? RoomType.Loot : RoomType.Puzzle;
            }
        }

        private static float MinEnemyRoomChance => 0.6f - (Perks.Has(Perk.Cartographer) ? 0.1f : 0f) - (DifficultyManager.Difficulty == Difficulty.Rookie ? 0.1f : 0f);
        private static float MaxEnemyRoomChance => 0.9f - (Perks.Has(Perk.Cartographer) ? 0.1f : 0f);

        /// <summary>
        /// Returns a difficulty based on the difficulty of the surrounding rooms<br/>
        /// Also assigns the room's pathAffinity (percentage of neighbours on the path)
        /// </summary>
        /// <param name="room"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static float GetDifficultyByConnections(Room room, List<Room> path)
        {
            float difficultySum = 0f;
            float numConnectedToPath = 0;
            foreach(Corridor corridor in room.connections)
            {
                Room other = corridor.roomA == room ? corridor.roomB : corridor.roomA;
                if(path.Contains(other)) numConnectedToPath++;
                difficultySum += other.difficulty;
            }

            room.pathAffinity = numConnectedToPath / room.connections.Count;
            return difficultySum / room.connections.Count;
        }

        /// <summary>
        /// Fills in the room types and difficulties of the rooms along the main path
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="rng"></param>
        private static void AssignAlongPath(Dungeon dungeon, Random rng)
        {
            //the -2 is there to account for the entrance and exit rooms
            int pathLength = dungeon.Path.Count - 2;
            for(int i = 0; i < pathLength; i++)
            {
                dungeon.Path[i].type = RoomType.Enemy;
                dungeon.Path[i].difficulty = (float) (i + 1) / pathLength;
            }

            foreach(int index in GetSpecialIndices(pathLength, rng))
            {
                //double random = rng.NextDouble();
                Vector2Int size = dungeon.Path[index].bounds.size;
                dungeon.Path[index].type = size.x > 8 && size.y > 8 ? RoomType.Puzzle : RoomType.Enemy; //random < SPECIAL_RATIO ? RoomType.Loot : RoomType.Puzzle;
            }
        }

        /// <summary>
        /// Generates indices for the special rooms (loot/puzzle) every 1 / (n + 1) rooms<br/>
        /// e.g. 1 room would be at 50%; 2 rooms at 33% and 67%; 3 rooms at 25%, 50% and 75%<br/>
        /// A random offset is applied to these percentages for variation
        /// </summary>
        /// <param name="pathLength"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static List<int> GetSpecialIndices(int pathLength, Random rng)
        {
            int specialCount = pathLength / 3;
            if(Perks.Has(Perk.Cartographer)) specialCount++;
            List<int> specialIndices = new List<int>();

            for(int i = 1; i < specialCount + 1; i++)
            {
                float fraction = i / (specialCount + 1f);
                float offset = (float)(rng.NextDouble() * 2d - 1d) * VARIATION;

                int index = Mathf.RoundToInt((fraction + offset) * pathLength);
                specialIndices.Add(Mathf.Clamp(index, 1, pathLength - 1));
            }

            return specialIndices;
        }
	}
}
