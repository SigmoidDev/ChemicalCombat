using Sigmoid.Utilities;
using Sigmoid.Rooms;

namespace Sigmoid.Game
{
    /// <summary>
    /// Stores death message or achievement-related stats that need to be kept across floors (but not runs)
    /// </summary>
	public class PersistentStats : Singleton<PersistentStats>
	{
		private int roomsCleared;
        public static int RoomsCleared => Instance.roomsCleared;

        private int puzzlesSolved;
        public static int PuzzlesSolved => Instance.puzzlesSolved;

        private int puzzlesEncountered;
        public static int PuzzlesEncountered => Instance.puzzlesEncountered;

        private int chestsOpened;
        public static int ChestsOpened => Instance.chestsOpened;
        public static void OpenChest() => Instance.chestsOpened++;

        private void Awake() => SceneLoader.Instance.OnSceneLoaded += RegisterSceneEvents;
        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded += RegisterSceneEvents;
        }

        private void RegisterSceneEvents(GameScene scene)
        {
            if(scene != GameScene.Labyrinth) return;

            //these all unsubscribe automatically when RoomGetter is destroyed on scene switch
            RoomGetter.Instance.OnRoomEntered += OnRoomEntered;
            RoomGetter.Instance.OnRoomCleared += OnRoomCleared;
            RoomGetter.Instance.OnPuzzleSolved += OnPuzzleSolved;
        }

        private void OnRoomEntered(PhysicalRoom room){ if(room is PuzzleRoom) puzzlesEncountered++; }
        private void OnRoomCleared(EnemyRoom room) => roomsCleared++;
        private void OnPuzzleSolved(PuzzleRoom room) => puzzlesSolved++;
    }
}
