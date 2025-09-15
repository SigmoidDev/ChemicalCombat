using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sigmoid.Generation
{
    public partial class Dungeon
    {
        public Vector2 EntrancePosition { get; private set; }
        public Vector2 ExitPosition { get; private set; }

        /// <summary>
        /// Creates an entrance to the dungeon based on some calculations
        /// </summary>
        /// <returns></returns>
		public Dungeon CreateEntrance()
		{
            int startX = Padding.left;
			RectOffset search = new RectOffset(startX, startX + 12, Size.y + Padding.bottom, Padding.bottom);
			(int, int) offset = EntranceGenerator.GetOptimalOffset(Map, search, false, 5f);

			int y = offset.Item1;
			for(int x = 4; x < offset.Item2; x++)
			{
				Map[x, y - 1] = true;
				Map[x, y] = true;
				Map[x, y + 1] = true;
			}

            EntrancePosition = new Vector2(20f, y);
            int checkHeight = y - (int) RenderOffset.y;
            FirstRoom = Rooms.Where(r => checkHeight >= r.bounds.yMin && checkHeight <= r.bounds.yMax).OrderBy(r => r.bounds.xMin).FirstOrDefault();
            if(FirstRoom == null) throw new InvalidDungeonException();
			return this;
		}

        /// <summary>
        /// Creates an exit from the dungeon similarly to the entrance
        /// </summary>
        /// <returns></returns>
		public Dungeon CreateExit()
		{
            int startX = Size.x + Padding.left;
			RectOffset search = new RectOffset(Size.x + Padding.left - 8, startX, Size.y + Padding.bottom, Padding.bottom);
			(int, int) offset = EntranceGenerator.GetOptimalOffset(Map, search, true, 5f);

			int y = offset.Item1;
            if(y == 0)
            {
                Debug.LogWarning("Dungeon generated with no exit!");
                return this;
            }

			for(int x = startX + 16; x >= offset.Item2; x--)
			{
				Map[x, y - 1] = true;
				Map[x, y] = true;
				Map[x, y + 1] = true;
			}

            ExitPosition = new Vector2(PaddedSize.x - 20f, y);
            int checkHeight = y - (int) RenderOffset.y;
            LastRoom = Rooms.Where(r => checkHeight >= r.bounds.yMin && checkHeight <= r.bounds.yMax).OrderByDescending(r => r.bounds.xMax).FirstOrDefault();
			return this;
		}

        /// <summary>
        /// Adds an AirlockDoor to prevent the player from leaving the map
        /// </summary>
        /// <returns></returns>
        public Dungeon BlockEntrance()
        {
            Vector2Int origin = new Vector2Int(Padding.left - 4, (int) EntrancePosition.y);
            Entrance = CreateArena(MapRenderer.Instance.EntranceRoom, origin, new Vector2(4f, -0.625f), FirstRoom);
            Rooms.Add(Entrance);

            Vector2 position = new Vector2(Padding.left - 6f, EntrancePosition.y - 0.625f);
            AirlockDoor door = FurniturePlacer.Instance.CreateDoor(position, false);
            door.Close();

            return this;
        }

        /// <summary>
        /// Creates a boss room at the far right side of the map
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        /*public Dungeon CreateBossRoom(ScriptableFloor floor)
        {
            Vector2Int origin = new Vector2Int(PaddedSize.x - Padding.right, (int) ExitPosition.y);
            BossRoom = CreateArena(floor.bossArena, origin, new Vector2(-2f, -0.625f), LastRoom);
            Rooms.Add(BossRoom);

            return this;
        }*/

        /// <summary>
        /// Creates the exit room as an extension of the boss room
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public Dungeon CreateExitRoom(ScriptableFloor floor)
        {
            Vector2Int origin = new Vector2Int(PaddedSize.x - Padding.right/* + floor.bossArena.size.x*/, (int) ExitPosition.y);
            ExitRoom = CreateArena(MapRenderer.Instance.ExitRoom, origin, new Vector2(2f, -0.625f), LastRoom);
            Rooms.Add(ExitRoom);

            //TODO: Change from LastRoom to BossRoom

            return this;
        }

        /// <summary>
        /// Constructs a room given a ScriptableArena and a position, and connects it to the previous room in the dungeon
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="origin"></param>
        /// <param name="doorOffset"></param>
        /// <param name="connectedRoom"></param>
        /// <returns></returns>
        public Room CreateArena(ScriptableArena arena, Vector2Int origin, Vector2 doorOffset, Room connectedRoom)
        {
            int maxX = MapRenderer.Instance.Dungeon.PaddedSize.x;
            int maxY = MapRenderer.Instance.Dungeon.PaddedSize.y;

            bool[,] walls = arena.Walls;
            for(int x = 0; x < arena.size.x; x++)
            {
                for(int y = 0; y < arena.size.y; y++)
                {
                    int xx = x - arena.entrance.x + origin.x;
                    int yy = y - arena.entrance.y + origin.y;

                    if(xx < 0 || xx >= maxX || yy < 0 || yy >= maxY) continue;
                    Map[xx, yy] = !walls[x, y];
                }
            }

            RectInt bounds = new RectInt(
                origin.x - arena.entrance.x - Padding.left - 1,
                origin.y - arena.entrance.y - Padding.bottom - 1,
                arena.size.x + 2,
                arena.size.y + 2
            );
            Room room = new Room(this, bounds);
            if(arena.centre != Vector2.zero) room.overrideCentre = origin - arena.entrance + arena.centre - MapRenderer.Instance.Dungeon.RenderOffset;

            HashSet<Vector2Int> tilesBetween = new HashSet<Vector2Int>();
            for(int x = -6; x <= 2; x++)
            {
                for(int y = -3; y <= 3; y++)
                {
                    int xx = x + origin.x;
                    int yy = y + origin.y;
                    tilesBetween.Add(new Vector2Int(xx, yy));
                }
            }

            AirlockDoor door = FurniturePlacer.Instance.CreateDoor(origin + doorOffset, false);
            Corridor corridor = new Corridor(connectedRoom, room, door, tilesBetween);
            connectedRoom.connections.Add(corridor);
            room.connections.Add(corridor);

            foreach(PrefabPlacement placement in arena.placements)
            {
                Vector2 position = placement.position + origin - arena.entrance;
                Quaternion rotation = placement.gameObject.transform.rotation;
                GameObject obj = Object.Instantiate(placement.gameObject, position, rotation);
                MapRenderer.Instance.OnRoomCreated += (r) => { if(r == room) obj.transform.SetParent(room.physicalRoom.transform); }; //unsubscribed automatically when unloading itself
            }

            return room;
        }
    }

    /// <summary>
    /// Handles the creation of the entrances and exits to the generated dungeon
    /// </summary>
	public static class EntranceGenerator
	{
		/// <summary>
		/// Defines the maximum length of the opening corridor
		/// </summary>
		private const int START_LIMIT = 12;
        /// <summary>
		/// Defines the maximum length of the opening corridor
		/// </summary>
		private const int END_LIMIT = 14;


		/// <summary>
		/// Evaluates the optimal y-offset for the dungeon based on the following:<br/>
		/// (corridorLimit - distanceOverMinimum) * numTilesAbove * numTilesBelow * normalDist(distanceFromCenter)
		/// </summary>
		/// <param name="map"></param>
		/// <param name="start"></param>
		/// <param name="centerPriority"></param>
		/// <returns></returns>
		public static (int, int) GetOptimalOffset(bool[,] map, RectOffset boundary, bool flipped, float centerPriority)
		{
            int startX = flipped ? boundary.right : boundary.left;

			Dictionary<int, int> distances = new Dictionary<int, int>();
			for(int y = boundary.bottom; y < boundary.top; y++)
			{
				int x = startX;
				while((!flipped && x < boundary.right) || (flipped && x > boundary.left))
				{
					if(flipped) x--;
                    else x++;

					bool bottom = map[x, y - 1];
					bool middle = map[x, y];
					bool top = map[x, y + 1];
					if(bottom && middle && top)
					{
						distances.Add(y, Mathf.Abs(x - startX));
						break;
					}
				}
			}

			if(distances.Count == 0) return (0, -1);
			KeyValuePair<int, int> minDistance = distances.Aggregate((a, b) => a.Value < b.Value ? a : b);
			Dictionary<int, int> evaluations = new Dictionary<int, int>();

			foreach(KeyValuePair<int, int> currentDistance in distances)
			{
				int excess = Mathf.Abs(minDistance.Value - currentDistance.Value);
				if(excess > (flipped ? END_LIMIT : START_LIMIT)) continue;

				int current = currentDistance.Key;
				int numAbove = 0;
				while(distances.TryGetValue(current, out int distance))
				{
					if(distance != currentDistance.Value) break;
					numAbove++;
					current++;
				}

				current = currentDistance.Key;
				int numBelow = 0;
				while(distances.TryGetValue(current, out int distance))
				{
					if(distance != currentDistance.Value) break;
					numBelow++;
					current--;
				}

				int sectionSize = boundary.bottom + boundary.top;
				float centerDist = Mathf.Abs(currentDistance.Key - sectionSize / 2);
				float distSquared = centerPriority * centerDist * centerDist;

				float deviationSquared = sectionSize * sectionSize;
				float contribution = Mathf.Exp(-distSquared / deviationSquared);

				float finalEvaluation = ((flipped ? END_LIMIT : START_LIMIT) + 1 - excess) * numAbove * numBelow * contribution;
				evaluations.Add(currentDistance.Key, (int) finalEvaluation);
			}

			int optimalRoute = evaluations.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
            int returnValue = flipped ? (startX - distances[optimalRoute]) : (distances[optimalRoute] + startX);
			return (optimalRoute, returnValue);
		}
	}
}
