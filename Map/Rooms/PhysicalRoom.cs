using Sigmoid.Generation;
using UnityEngine;

namespace Sigmoid.Rooms
{
    /// <summary>
    /// Base class for any physically instantiated room, with methods for entering and exiting
    /// </summary>
	public abstract class PhysicalRoom : MonoBehaviour
	{
		[SerializeField] private BoxCollider2D trigger;
        public Room Room { get; private set; }

        public virtual PhysicalRoom Initialise(Room room)
		{
			Room = room;
            Room.physicalRoom = this;

			trigger.size = room.bounds.size - Vector2.one * 2f;
            return this;
		}

        /// <summary>
        /// Called when the player first enters the room
        /// </summary>
        protected abstract void OnEntered();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if(!other.CompareTag("Player")) return;
			if(Room.state != RoomState.Undiscovered) return;
			Room.state = RoomState.Entered;
            RoomGetter.Instance.EnterRoom(this);
            OnEntered();
		}

        /// <summary>
        /// Shuts the connected AirlockDoors
        /// </summary>
        public void LockDoors()
        {
            foreach(Corridor corridor in Room.connections)
				corridor.door.Close();
        }

        /// <summary>
        /// Opens the connected AirlockDoors
        /// </summary>
        public void UnlockDoors()
        {
            foreach(Corridor corridor in Room.connections)
				corridor.door.Open();
        }
	}
}
