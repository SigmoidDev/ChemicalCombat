using System.Collections.Generic;
using Sigmoid.Puzzles;
using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Generation
{
    /// <summary>
    /// A full map, containing all its settings, rooms, and the path from start to finish
    /// </summary>
	public partial class Dungeon
	{
        public readonly int Seed;
		public readonly Vector2Int Size;
		public readonly RectOffset Padding;
        public Vector2Int PaddedSize => new Vector2Int(Size.x + Padding.left + Padding.right, Size.y + Padding.bottom + Padding.top);

		public Vector2 PaddingOffset => new Vector2(Padding.left, Padding.bottom);
		public Vector2 RenderOffset => new Vector2(Padding.left, Padding.bottom);

        public Room Entrance { get; set; }
        public Room FirstRoom { get; set; }
        public Room LastRoom { get; set; }
        //public Room BossRoom { get; set; }
        public Room ExitRoom { get; set; }

        public List<Room> Path { get; set; }
		public List<Room> Rooms { get; set; }
		public bool[,] Map { get; set; }

		public Dungeon(int seed, Vector2Int size, RectOffset padding)
		{
            Seed = seed;
			Size = size;
			Padding = padding;
			Map = new bool[PaddedSize.x, PaddedSize.y];

			Rooms = new List<Room>();
			Corridors = new List<Corridor>();
			NewCorridors = new Queue<CorridorState>();
		}

		public Room FindByNode(RoomNode node)
		{
			foreach(Room room in Rooms)
			{
				if(room.bounds.center == node.interior.center
				&& room.bounds.size == node.interior.size)
					return room;
			}
			return null;
		}
	}

	public class Room
	{
		public readonly Dungeon dungeonReference;
        public int Seed
        {
            get
            {
                int value = connections.Count;
                value = value * 31 + (int) bounds.center.x * 31 % 10000;
                value = value * 37 + (int) bounds.center.y * 37 % 10000;
                value = value * 41 + bounds.xMin * 41 % 10000;
                value = value * 47 + bounds.xMax * 47 % 10000;
                value = value * 51 + bounds.yMin * 51 % 10000;
                value = value * 57 + bounds.yMax * 57 % 10000;
                return Mathf.Abs(value) % 10000;
            }
        }

        public readonly RectInt bounds;
        public readonly Rect interior;
        public readonly RoomSize SizeCategory;
		public int Area => bounds.width * bounds.height;
		public int Size => (bounds.width + bounds.height) / 2;
		public float XBalance => (float) bounds.width / bounds.height;
		public float YBalance => (float) bounds.height / bounds.width;
		public float Balance => Mathf.Max(XBalance, YBalance);

		public Vector2 TopLeft => new Vector2(bounds.xMin + 2, bounds.yMax - 2);
		public Vector2 TopRight => new Vector2(bounds.xMax - 2, bounds.yMax - 2);
		public Vector2 BottomLeft => new Vector2(bounds.xMin + 2, bounds.yMin + 2);
		public Vector2 BottomRight => new Vector2(bounds.xMax - 2, bounds.yMin + 2);

        public RoomType type;
		public RoomState state;
		public List<Corridor> connections;
        public float pathAffinity;
		public float difficulty;
		public PhysicalRoom physicalRoom;
        public Vector2? overrideCentre;
        public PuzzleType puzzleType;

		public Room(Dungeon dungeonReference, RectInt bounds)
		{
            type = RoomType.Unassigned;
            state = RoomState.Undiscovered;
			connections = new List<Corridor>();
            pathAffinity = 0f;
            difficulty = 1f;

			this.dungeonReference = dungeonReference;
			this.bounds = bounds;
            interior = new Rect(
                bounds.xMin + 3f + dungeonReference.RenderOffset.x,
                bounds.yMin + 3f + dungeonReference.RenderOffset.y,
                bounds.width - 6f,
                bounds.height - 7f
            );

            if(Area > 576 || (Area > 512 && Balance > 1.3f)) SizeCategory = RoomSize.Huge;
			else if(Area > 384 || (Area > 320 && Balance > 1.3f)) SizeCategory = RoomSize.Large;
			else if(Area > 256 || (Area > 192 && Balance > 1.2f)) SizeCategory = RoomSize.Medium;
		}

        /// <summary>
        /// Draws a box around the room's bounds using Debug.DrawLine
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="colour"></param>
        /// <param name="duration"></param>
		public void DebugDraw(Vector2 offset, Color colour, float duration)
		{
			Debug.DrawLine(TopLeft + offset, TopRight + offset, colour, duration);
			Debug.DrawLine(TopRight + offset, BottomRight + offset, colour, duration);
			Debug.DrawLine(BottomRight + offset, BottomLeft + offset, colour, duration);
			Debug.DrawLine(BottomLeft + offset, TopLeft + offset, colour, duration);
		}
	}

	public enum RoomState
	{
		Undiscovered,
		Entered,
		Cleared
	}

    public enum RoomType
    {
        Unassigned,
        Enemy,
        Loot,
        Puzzle,
        Respite,
        Boss,
        Portal
    }

    public enum RoomSize
    {
        Small = 0,
        Medium = 1,
        Large = 2,
        Huge = 3
    }

    /// <summary>
    /// Extension methods for the RoomType enum that allow for categorisation
    /// </summary>
    public static class RoomTypeClassifier
    {
        /// <summary>
        /// A standard room refers to an enemy, loot, or puzzle room that is procedurally generated
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStandard(this RoomType type) => type == RoomType.Enemy || type == RoomType.Loot || type == RoomType.Puzzle;
        /// <summary>
        /// A custom room refers to anything non-standard, i.e. a prebuilt corridor, boss room, the exit room, etc.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCustom(this RoomType type) => !IsStandard(type);
    }
}
