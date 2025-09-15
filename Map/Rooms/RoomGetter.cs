using System;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Rooms
{
    /// <summary>
    /// Class for determining things to do with the whereabouts of the player
    /// </summary>
	public class RoomGetter : Singleton<RoomGetter>
	{
        /// <summary>
        /// Attempts to get the PhysicalRoom that the player is in, returning whether or not a room was found
        /// </summary>
        /// <param name="playerRoom"></param>
        /// <returns></returns>
        public static bool TryGetRoom(out PhysicalRoom playerRoom)
        {
            playerRoom = null;
            if(SceneLoader.Instance.CurrentScene != GameScene.Labyrinth) return false;

            foreach(Room room in MapRenderer.Instance.Dungeon.Rooms)
            {
                if(room.state == RoomState.Entered)
                {
                    playerRoom = room.physicalRoom;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to find the containing room at a given point, returning whether or not the point is inside a room
        /// </summary>
        /// <param name="position"></param>
        /// <param name="foundRoom"></param>
        /// <returns></returns>
        public static bool TryGetByPosition(Vector2 position, out PhysicalRoom foundRoom)
        {
            foundRoom = null;
            if(SceneLoader.Instance.CurrentScene != GameScene.Labyrinth) return false;

            foreach(Room room in MapRenderer.Instance.Dungeon.Rooms)
            {
                Rect bounds = room.interior;
                if(position.x > bounds.xMin && position.x < bounds.xMax
                && position.y > bounds.yMin && position.y < bounds.yMax)
                {
                    foundRoom = room.physicalRoom;
                    return true;
                }
            }

            return false;
        }

        public event Action<PhysicalRoom> OnRoomEntered;
        public void EnterRoom(PhysicalRoom room) => OnRoomEntered?.Invoke(room);

        public event Action<EnemyRoom> OnRoomCleared;
        public void ClearRoom(EnemyRoom room) => OnRoomCleared?.Invoke(room);

        public event Action<PuzzleRoom> OnPuzzleSolved;
        public void SolvePuzzle(PuzzleRoom room) => OnPuzzleSolved?.Invoke(room);
    }
}
