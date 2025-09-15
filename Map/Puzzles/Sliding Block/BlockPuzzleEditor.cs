#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Sigmoid.Puzzles
{
    [CustomEditor(typeof(ScriptableBlockPuzzle))]
	public class BlockPuzzleEditor : Editor
	{
        private GUIStyle toggleStyle;
        private void ResetStyles()
        {
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            toggleStyle.fixedWidth = 14;
            toggleStyle.fixedHeight = 14;
        }

        public override void OnInspectorGUI()
        {
            BlockPuzzle puzzle = ((ScriptableBlockPuzzle) target).puzzle;
            if(puzzle.walls == null || puzzle.walls.Length != 49) puzzle.walls = new bool[49];

            ResetStyles();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();

            for(int y = 6; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                for(int x = 0; x < 7; x++)
                {
                    if(x == puzzle.start.x && y == puzzle.start.y) GUI.backgroundColor = Color.green;
                    else if(x == puzzle.exit.x && y == puzzle.exit.y) GUI.backgroundColor = Color.red;
                    else GUI.backgroundColor = Color.white;

                    int i = 7 * y + x;
                    puzzle.walls[i] = GUILayout.Toggle(puzzle.walls[i], "", toggleStyle);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            GUILayout.Space(12);

            puzzle.start = EditorGUILayout.Vector2IntField("Start:", puzzle.start);
            puzzle.exit = EditorGUILayout.Vector2IntField("Exit:", puzzle.exit);

            if(GUILayout.Button("Rotate Clockwise"))
                ((ScriptableBlockPuzzle) target).puzzle = puzzle.Rotate();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
	}
}
#endif