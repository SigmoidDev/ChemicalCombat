using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Class for pathfinding from any given Room to another
    /// </summary>
	public static class RoomTraverser
	{
        /// <summary>
        /// Runs a DFS on the rooms' connections to find the path to the target room provided
        /// </summary>
        /// <param name="roomA"></param>
        /// <param name="roomB"></param>
        /// <returns></returns>
        public static List<Room> FindPath(Room roomA, Room roomB) => Traverse(roomA, roomB, new List<Room>(), new HashSet<Room>());
        private static List<Room> Traverse(Room current, Room target, List<Room> path, HashSet<Room> explored)
        {
            path.Add(current);
            if(current == target) return path;
            explored.Add(current);

            foreach(Corridor corridor in current.connections)
            {
                Room other = corridor.roomA == current ? corridor.roomB : corridor.roomA;
                if(explored.Contains(other)) continue;

                List<Room> results = Traverse(other, target, new List<Room>(path), explored);
                if(results != null) return results;
            }

            return null;
        }

        /// <summary>
        /// Draws boxes around the rooms on the path, coloured from red to green over the length
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        public static void DebugPath(List<Room> path, Vector2 offset, float duration)
        {
            for(int i = 0; i < path.Count; i++)
            {
                float fraction = (float) (i + 1) / path.Count;
                Color colour = new Color(fraction, 1f - fraction, 0f);
                path[i].DebugDraw(offset, colour, duration);
            }
        }
	}
}
