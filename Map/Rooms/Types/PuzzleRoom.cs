using System.Linq;
using Sigmoid.Generation;
using Sigmoid.Puzzles;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Rooms
{
	public class PuzzleRoom : PhysicalRoom
	{
        private Puzzle puzzle;
        public override PhysicalRoom Initialise(Room room)
        {
            base.Initialise(room);

            Puzzle prefab = FloorManager.Instance.Floor.puzzleObjects.First(p => p.type == room.puzzleType).prefab;
            puzzle = Instantiate(prefab, room.bounds.center + MapRenderer.Instance.Dungeon.RenderOffset - 0.5f * Vector2.up, Quaternion.identity, transform);
            puzzle.OnComplete += Complete;
            puzzle.Initialise(Room.Seed);

            return this;
        }

        private void Complete(bool successful)
        {
            puzzle.OnComplete -= Complete;
            Room.state = RoomState.Cleared;

            if(successful) RoomGetter.Instance.SolvePuzzle(this);
        }

        protected override void OnEntered()
        {
            
        }
    }
}
