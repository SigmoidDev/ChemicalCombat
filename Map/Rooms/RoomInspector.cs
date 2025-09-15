#if UNITY_EDITOR
using Sigmoid.Generation;
using UnityEditor;

namespace Sigmoid.Rooms
{
    [CustomEditor(typeof(PhysicalRoom))]
    public class RoomInspector : Editor
    {
        private static bool showConnections;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Room room = ((PhysicalRoom)target).Room;
            if(room == null)
            {
                EditorGUILayout.LabelField("Room not assigned. Enter play mode to view properties.");
                return;
            }

            EditorGUILayout.EnumPopup("Type: ", room.type);
            if(room.type == RoomType.Puzzle) EditorGUILayout.EnumPopup("Puzzle: ", room.puzzleType);

            EditorGUILayout.EnumPopup("State: ", room.state);
            EditorGUILayout.FloatField("Difficulty", room.difficulty * 100);
            EditorGUILayout.Space();

            EditorGUILayout.IntField("Seed: ", room.Seed);
            EditorGUILayout.IntField("Area: ", room.Area);
            EditorGUILayout.IntField("Size: ", room.Size);
            EditorGUILayout.EnumPopup("Size: ", room.SizeCategory);
            EditorGUILayout.RectIntField("Bounds: ", room.bounds);
            EditorGUILayout.RectField("Interior: ", room.interior);

            showConnections = EditorGUILayout.Foldout(showConnections, "Connections");
            if(showConnections)
            {
                foreach(Corridor corridor in room.connections)
                {
                    Room other = corridor.roomA == room ? corridor.roomB : corridor.roomA;
                    EditorGUILayout.ObjectField("Room:", other.physicalRoom, typeof(PhysicalRoom), true);
                }
            }
        }
    }

    [CustomEditor(typeof(EnemyRoom))]  public class EnemyRoomInspector  : RoomInspector {}
    [CustomEditor(typeof(LootRoom))]   public class LootRoomInspector   : RoomInspector {}
    [CustomEditor(typeof(PuzzleRoom))] public class PuzzleRoomInspector : RoomInspector {}
    [CustomEditor(typeof(BossRoom))]   public class BossRoomInspector   : RoomInspector {}
    [CustomEditor(typeof(PresetRoom))] public class PresetRoomInspector : RoomInspector {}
}
#endif