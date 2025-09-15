using UnityEngine;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Class to handle the generation of dungeons through BSP
    /// </summary>
	public static class DungeonGenerator
	{
		/// <summary>
		/// Generates a map based on the parameters provided
		/// </summary>
		/// <param name="size"></param>
		/// <param name="padding"></param>
		/// <param name="seed"></param>
		/// <param name="depth"></param>
		/// <param name="variation"></param>
		/// <param name="smoothness"></param>
		/// <param name="underpopulationNumber"></param>
		/// <param name="overpopulationNumber"></param>
		/// <returns></returns>
		public static Dungeon GenerateDungeon(Vector2Int size, RectOffset padding, int seed, int depth, int wallThickness, int variation, int smoothness, int underpopulationNumber, int overpopulationNumber)
		{
			RoomNode parent = GenerateTree(size, depth, seed);
			Dungeon dungeon = GenerateMap(seed, parent, size, padding, wallThickness);

			dungeon = CellularAutomata.Noisify(dungeon, variation, seed);
			dungeon = RoomConnector.AddConnections(dungeon, parent, depth);
			dungeon = CellularAutomata.Smoothen(dungeon, smoothness, underpopulationNumber, overpopulationNumber);

			return dungeon;
		}



		/// <summary>
		/// Generates a new dungeon with the size, recursion depth, and seed provided<br/>
		/// Depth is how many subdivisions each room makes
		/// </summary>
		/// <param name="depth"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		private static RoomNode GenerateTree(Vector2Int size, int depth, int seed)
		{
			RectInt bounds = new RectInt(0, 0, size.x, size.y);
			System.Random rng = new System.Random(seed);
			RoomNode dungeon = new RoomNode(bounds, depth, depth, rng);
			return dungeon;
		}



		/// <summary>
		/// Converts a binary tree into a 2D boolean array representing the map<br/>
		/// true corresponds to walkable space, whereas false is for walls
		/// </summary>
		/// <param name="dungeon"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="padding"></param>
		/// <returns></returns>
		private static Dungeon GenerateMap(int seed, RoomNode parent, Vector2Int size, RectOffset padding, int wallThickness)
		{
			Dungeon dungeon = new Dungeon(seed, size, padding);

			foreach(RoomNode room in parent.GetRooms())
			{
				int left = room.interior.x + padding.left + wallThickness;
				int right = room.interior.x + room.interior.width + padding.left - wallThickness;
				int bottom = room.interior.y + padding.bottom + wallThickness;
				int top = room.interior.y + room.interior.height + padding.bottom - wallThickness;

				dungeon.Map = ClearRoom(dungeon.Map, left, right, bottom, top);
				dungeon.Rooms.Add(new Room(dungeon, room.interior));
			}

			return dungeon;
		}



		/// <summary>
		/// Removes the space from the map where there is a room
		/// </summary>
		/// <param name="map"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		private static bool[,] ClearRoom(bool[,] map, int left, int right, int bottom, int top)
		{
			for(int x = left; x < right; x++)
			{
				for(int y = bottom; y < top; y++)
				{
					bool isTopLeft = x == left && y == top - 1;
					bool isTopRight = x == right - 1 && y == top - 1;
					bool isBottomLeft = x == left && y == bottom;
					bool isBottomRight = x == right - 1 && y == bottom;
					if(!isTopLeft && !isTopRight && !isBottomLeft && !isBottomRight) map[x, y] = true;
				}
			}

			return map;
		}
	}
}
