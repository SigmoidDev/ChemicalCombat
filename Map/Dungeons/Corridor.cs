using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Generation
{
    public partial class Dungeon
    {
        public List<Corridor> Corridors { get; set; }
		public Queue<CorridorState> NewCorridors { get; set; }

        /// <summary>
        /// Loops through every CorridorState in NewCorridors and creates a corridor with a door
        /// </summary>
        public void CreateCorridors()
		{
			while(NewCorridors.Count > 0)
			{
				CorridorState state = NewCorridors.Dequeue();
                Vector2 offset = !state.isVertical ? new Vector2(-0.5f, 0f) : Vector2.zero;
				AirlockDoor door = FurniturePlacer.Instance.CreateDoor(state.doorCoords + offset, state.isVertical);

				Corridor corridor = new Corridor(state.roomA, state.roomB, door, state.tiles);
				state.roomA.connections.Add(corridor);
				state.roomB.connections.Add(corridor);
			}
		}
    }

    /// <summary>
    /// Represents a connection between two rooms, including the door that separates them
    /// </summary>
	public class Corridor
	{
		public readonly Room roomA;
		public readonly Room roomB;
		public readonly AirlockDoor door;
        public readonly HashSet<Vector2Int> tiles;

		public Corridor(Room roomA, Room roomB, AirlockDoor door, HashSet<Vector2Int> tiles)
		{
			this.roomA = roomA;
			this.roomB = roomB;
			this.door = door;
            this.tiles = tiles;
		}
	}
}
