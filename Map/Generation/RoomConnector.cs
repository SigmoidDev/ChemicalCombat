using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Provides methods to connect nearby rooms and dig out corridors
    /// </summary>
	public static class RoomConnector
	{
		/// <summary>
		/// Adds the required connections for each layer of the tree
		/// </summary>
		/// <param name="map"></param>
		/// <param name="dungeon"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		public static Dungeon AddConnections(Dungeon dungeon, RoomNode parent, int depth)
		{
            for(int layer = 0; layer < depth; layer++)
            {
                foreach(Connection connection in parent.GetConnections(depth - layer - 1))
                {
                    Room roomA = dungeon.FindByNode(connection.start);
                    Room roomB = dungeon.FindByNode(connection.end);
                    if(roomA == null || roomB == null) continue;

                    dungeon = ClearCorridor(dungeon, connection, roomA, roomB);
                }
            }
			return dungeon;
		}

        /// <summary>
        /// What fraction of the way between rooms A and B a door should be placed
        /// </summary>
		public const float CENTER_FOCUS = 0.4f;

		/// <summary>
        /// Somehow makes a path in the dungeon's map based on the connection provided<br/>
		/// Honestly I have no idea how it works (it came from a Sebastian Lague video)
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="connection"></param>
        /// <param name="actualRoomA"></param>
        /// <param name="actualRoomB"></param>
        /// <returns></returns>
		public static Dungeon ClearCorridor(Dungeon dungeon, Connection connection, Room actualRoomA, Room actualRoomB)
		{
			Vector2 centerA = connection.start.interior.center + dungeon.PaddingOffset;
			Vector2 centerB = connection.end.interior.center + dungeon.PaddingOffset;

			Vector2 displacement = centerB - centerA;
			bool isVertical = Mathf.Abs(displacement.x) < Mathf.Abs(displacement.y);

			if(isVertical)
			{
				centerA.x = Mathf.Lerp(centerA.x, centerB.x, CENTER_FOCUS);
				centerB.x = Mathf.Lerp(centerA.x, centerB.x, 1f - CENTER_FOCUS);
			}
			else
			{
				centerA.y = Mathf.Lerp(centerA.y, centerB.y, CENTER_FOCUS);
				centerB.y = Mathf.Lerp(centerA.y, centerB.y, 1f - CENTER_FOCUS);
			}

			List<Vector2> path = new List<Vector2>();
            HashSet<Vector2Int> changes = new HashSet<Vector2Int>();

            //two traces need to be done because the former checks the surrounding tiles (which the latter changes)
			bool pathIsVertical = TracePath(centerA, centerB, (x, y) => { if(!dungeon.Map[x, y]) path.Add(new Vector2(x, y)); });
            TracePath(centerA, centerB, (x, y) => (dungeon.Map, changes) = Bulldoze(dungeon.Map, x, y, 1, changes));

			if(path.Count != 0)
			{
				Vector2 center = path[path.Count / 2] + new Vector2(0.5f, -0.625f);
				CorridorState createdCorridor = new CorridorState(actualRoomA, actualRoomB, center, pathIsVertical, changes);
				dungeon.NewCorridors.Enqueue(createdCorridor);
			}
			return dungeon;
		}

        /// <summary>
        /// Traces a line from A to B, performing some action on each position
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="action"></param>
        /// <returns></returns>
		private static bool TracePath(Vector2 start, Vector2 end, Action<int, int> action)
		{
			int x = (int) start.x;
			int y = (int) start.y;

			int dx = (int) (end.x - start.x);
			int dy = (int) (end.y - start.y);

			int horizontal = Mathf.Abs(dx);
			int vertical = Mathf.Abs(dy);
			bool inverted = horizontal < vertical;

			int longest = inverted ? vertical : horizontal;
			int shortest = inverted ? horizontal : vertical;

			int step = inverted ? Math.Sign(dy) : Math.Sign(dx);
			int gradStep = inverted ? Math.Sign(dx) : Math.Sign(dy);



			int gradAcc = longest / 2;
			for(int i = 0; i < longest; i++)
			{
				action?.Invoke(x, y);

				if(inverted) y += step;
				else x += step;

				gradAcc += shortest;
				if(gradAcc >= longest)
				{
					if(inverted) x += gradStep;
					else y += gradStep;
				}
				gradAcc -= longest;
			}

			return inverted;
		}

		/// <summary>
        /// Drills a square hole of size radius * radius around the given (x, y)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="changes"></param>
        /// <returns></returns>
		public static (bool[,], HashSet<Vector2Int>) Bulldoze(bool[,] map, int x, int y, int radius, HashSet<Vector2Int> changes)
		{
			for(int xx = -radius - 2; xx <= radius + 2; xx++)
			{
				for(int yy = -radius - 2; yy <= radius + 2; yy++)
				{
					int newX = Mathf.Max(0, x + xx);
					int newY = Mathf.Max(0, y + yy);
                    changes.Add(new Vector2Int(newX, newY));

                    if(xx <= -radius - 1 || xx >= radius + 1
                    || yy <= -radius - 1 || yy >= radius + 1) continue;
					map[newX, newY] = true;
				}
			}
			return (map, changes);
		}
	}

	/// <summary>
	/// The half of RoomNode that holds the logic for connecting rooms to other rooms
	/// </summary>
	public partial class RoomNode
	{
		/// <summary>
		/// Gets a list of tuples containing all rooms that need to be connected
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public List<Connection> GetConnections(int level)
		{
			List<Connection> connections = new List<Connection>();
            if(this.level == level)
            {
                List<Connection> possible = FindShortestConnection();
                if(possible != null) connections.Add(possible[0]);
                else return null;
            }
            else
            {
                List<Connection> roomAConns = roomA.GetConnections(level);
                List<Connection> roomBConns = roomB.GetConnections(level);
                if(roomAConns != null) connections.AddRange(roomAConns);
                if(roomBConns != null) connections.AddRange(roomBConns);

                if(roomAConns == null || roomBConns == null)
                {
                    List<Connection> possible = FindShortestConnection();
                    if(possible != null && possible.Count > 1) connections.Add(possible[1]);
                }
            }
			return connections;
		}

        /// <summary>
        /// The maximum length that any corridor can be before it is discarded
        /// </summary>
		public const float MAX_LENGTH = 20f;
        /// <summary>
        /// The maximum allowed angle between any two connected rooms
        /// </summary>
        public const float MAX_ANGLE = 30f;

		/// <summary>
		/// Finds the shortest possible paths between all of the subrooms of A and B
		/// </summary>
		/// <returns></returns>
		private List<Connection> FindShortestConnection()
		{
            if(roomA == null || roomB == null) return null;

			Dictionary<Connection, float> distances = new Dictionary<Connection, float>(new ConnectionComparer());
			List<RoomNode> roomsOfA = roomA.GetRooms();
			List<RoomNode> roomsOfB = roomB.GetRooms();

			foreach(RoomNode room in roomsOfA)
			{
				if(room.interior.width == 0) continue;
				foreach(RoomNode other in roomsOfB)
				{
					if(other.interior.width == 0) continue;

					float distance = GetDistance(room.interior, other.interior);
					if(distance > MAX_LENGTH || AreDisconnected(room.interior, other.interior)) continue;

                    Vector2 offset = other.interior.center - room.interior.center;
					float angleTo = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
					float absAngle = Mathf.Abs(angleTo >= 90f ? angleTo - 180f : angleTo <= -90f ? angleTo + 180f : angleTo); //Adjust it to be 0 to 90
					if(absAngle > MAX_ANGLE && absAngle < 90f - MAX_ANGLE) continue;

					Connection connection = new Connection(room, other);
					distances[connection] = distance;
				}
			}

			if(distances.Count == 0) return null;
            return distances.OrderBy(pair => pair.Value)
                .ThenBy(pair => Vector2.Distance(pair.Key.start.interior.center, pair.Key.end.interior.center))
                .Select(pair => pair.Key)
                .ToList();
		}

		/// <summary>
		/// Returns the shortest distance between rooms A and B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float GetDistance(RectInt a, RectInt b)
		{
			int aLeft = a.xMin;
			int aRight = a.xMax;
			int aBottom = a.yMin;
			int aTop = a.yMax;

			int bLeft = b.xMin;
			int bRight = b.xMax;
			int bBottom = b.yMin;
			int bTop = b.yMax;

			bool onLeft = bRight < aLeft;
			bool onRight = aRight < bLeft;
			bool onBottom = bTop < aBottom;
			bool onTop = aTop < bBottom;

			//help my head hurts
			if(onTop && onLeft) return Vector2.Distance(new Vector2(aLeft, aTop), new Vector2(bRight, bBottom));
			if(onBottom && onLeft) return Vector2.Distance(new Vector2(aLeft, aBottom), new Vector2(bRight, bTop));
			if(onTop && onRight) return Vector2.Distance(new Vector2(aRight, aBottom), new Vector2(bLeft, bTop));
			if(onBottom && onRight) return Vector2.Distance(new Vector2(aRight, aTop), new Vector2(bLeft, bBottom));
			if(onLeft) return aLeft - bRight;
			if(onRight) return bLeft - aRight;
			if(onTop) return bBottom - aTop;
			if(onBottom) return aBottom - bTop;
			return 0;
		}

        public const int ROOM_CONNECTED_THRESHOLD = 4;
        public static bool AreDisconnected(RectInt rectA, RectInt rectB)
        {
            return
            (  rectB.xMin - rectA.xMax > -ROOM_CONNECTED_THRESHOLD
            || rectA.xMin - rectB.xMax > -ROOM_CONNECTED_THRESHOLD)
            &&(rectB.yMin - rectA.yMax > -ROOM_CONNECTED_THRESHOLD
            || rectA.yMin - rectB.yMax > -ROOM_CONNECTED_THRESHOLD);
        }
	}

	/// <summary>
	/// Represents a possible connection between 2 rooms
	/// </summary>
	public class Connection
	{
		public readonly RoomNode start;
		public readonly RoomNode end;

		public Connection(RoomNode start, RoomNode end)
		{
			this.start = start;
			this.end = end;
		}
	}

	/// <summary>
	/// Allows for connections to be compared where A->B is the same as B->A
	/// </summary>
	public class ConnectionComparer : IEqualityComparer<Connection>
    {
        public bool Equals(Connection a, Connection b) => (a.start == b.start && a.end == b.end) || (a.start == b.end && b.start == a.end);
        public int GetHashCode(Connection obj) => base.GetHashCode();
    }

	/// <summary>
	/// Represents a corridor that needs to be created, allowing for the door to be placed with the correct orientation
	/// </summary>
	public class CorridorState
	{
		public readonly Room roomA;
		public readonly Room roomB;
		public readonly Vector2 doorCoords;
		public readonly bool isVertical;
        public readonly HashSet<Vector2Int> tiles;

		public CorridorState(Room roomA, Room roomB, Vector2 doorCoords, bool isVertical, HashSet<Vector2Int> tiles)
		{
			this.roomA = roomA;
			this.roomB = roomB;
			this.doorCoords = doorCoords;
			this.isVertical = isVertical;
            this.tiles = tiles;
		}
	}
}
