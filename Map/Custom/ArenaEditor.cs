#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Sigmoid.Generation
{
    [CustomEditor(typeof(ScriptableArena))]
    public class ArenaEditor : Editor
    {
        private static bool showingObjects;
        private static bool showingDangerous;

        private GUIStyle toggleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle deleteStyle;
        private void ResetStyles()
        {
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            toggleStyle.fixedWidth = 14;
            toggleStyle.fixedHeight = 14;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedWidth = 24;
            buttonStyle.fixedHeight = 24;

            deleteStyle = new GUIStyle(GUI.skin.button);
            deleteStyle.fixedWidth = EditorGUIUtility.singleLineHeight * 2;
            deleteStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnInspectorGUI()
        {
            ScriptableArena arena = (ScriptableArena) target;
            if(arena.walls == null)
            {
                arena.size = new Vector2Int(6, 6);
                arena.walls = new bool[36];
            }
            if(arena.placements == null)
            {
                arena.placements = new List<PrefabPlacement>();
            }

            ResetStyles();

            EditorGUI.BeginChangeCheck();
            arena.entrance = EditorGUILayout.Vector2IntField("Entrance", arena.entrance);
            arena.entrance.x = Mathf.Clamp(arena.entrance.x, 0, arena.size.x - 1);
            arena.entrance.y = Mathf.Clamp(arena.entrance.y, 0, arena.size.y - 1);

            arena.exit = EditorGUILayout.Vector2IntField("Exit", arena.exit);
            arena.exit.x = Mathf.Clamp(arena.exit.x, 0, arena.size.x - 1);
            arena.exit.y = Mathf.Clamp(arena.exit.y, 0, arena.size.y - 1);

            arena.centre = EditorGUILayout.Vector2Field("Centre", arena.centre);
            arena.centre.x = Mathf.Clamp(arena.centre.x, 0, arena.size.x - 1);
            arena.centre.y = Mathf.Clamp(arena.centre.y, 0, arena.size.y - 1);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.BeginVertical();
            AddRowButtons(arena, true);
            EditorGUILayout.BeginHorizontal();
            AddColButtons(arena, true);
            DrawGrid(arena);
            AddColButtons(arena, false);
            EditorGUILayout.EndHorizontal();
            AddRowButtons(arena, false);
            EditorGUILayout.EndVertical();

            GUILayout.Space(12);
            showingObjects = EditorGUILayout.Foldout(showingObjects, "Objects");
            if(showingObjects)
            {
                EditorGUI.BeginChangeCheck();
                foreach(PrefabPlacement placement in arena.placements.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    placement.gameObject = (GameObject) EditorGUILayout.ObjectField("Object: ", placement.gameObject, typeof(GameObject), false);
                    placement.position = EditorGUILayout.Vector2Field("Position: ", placement.position);
                    EditorGUILayout.EndVertical();

                    //holy shit that makes a fire bin emoji
                    if(GUILayout.Button("___\n[#]\n", deleteStyle))
                        arena.placements.Remove(placement);
                    EditorGUILayout.EndHorizontal();
                }
                if(GUILayout.Button("New Object"))
                    arena.placements.Add(new PrefabPlacement());
                

                if(EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                }
            }

            showingDangerous = EditorGUILayout.Foldout(showingDangerous, "Danger Zone");
            if(showingDangerous)
            {
                bool modified = false;
                if(GUILayout.Button("Set All Solid"))
                {
                    modified = true;
                    for(int i = 0; i < arena.size.x * arena.size.y; i++)
                        arena.walls[i] = true;
                }
                if(GUILayout.Button("Set All Empty"))
                {
                    modified = true;
                    for(int i = 0; i < arena.size.x * arena.size.y; i++)
                        arena.walls[i] = false;
                }
                if(GUILayout.Button("Force Serialization"))
                {
                    modified = true;
                    arena.Walls = arena.Walls;
                }
                if(modified)
                {
                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void AddRowButtons(ScriptableArena arena, bool top)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("+", buttonStyle))
            {
                bool[,] oldArena = arena.Walls;
                bool[,] newArena = new bool[arena.size.x, arena.size.y + 1];
                for(int x = 0; x < arena.size.x; x++)
                {
                    for(int y = 0; y < arena.size.y; y++)
                    {
                        newArena[x, top ? y : y + 1] = oldArena[x, y];
                    }
                }

                arena.Walls = newArena;
            }
            if(GUILayout.Button("-", buttonStyle))
            {
                bool[,] oldArena = arena.Walls;
                bool[,] newArena = new bool[arena.size.x, arena.size.y - 1];
                for(int x = 0; x < arena.size.x; x++)
                {
                    for(int y = top ? 0 : 1; y < arena.size.y - (top ? 1 : 0); y++)
                    {
                        newArena[x, top ? y : y - 1] = oldArena[x, y];
                    }
                }

                arena.Walls = newArena;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void AddColButtons(ScriptableArena arena, bool left)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("+", buttonStyle))
            {
                bool[,] oldArena = arena.Walls;
                bool[,] newArena = new bool[arena.size.x + 1, arena.size.y];
                for(int x = 0; x < arena.size.x; x++)
                {
                    for(int y = 0; y < arena.size.y; y++)
                    {
                        newArena[left ? x + 1 : x, y] = oldArena[x, y];
                    }
                }

                arena.Walls = newArena;
            }
            if(GUILayout.Button("-", buttonStyle))
            {
                bool[,] oldArena = arena.Walls;
                bool[,] newArena = new bool[arena.size.x - 1, arena.size.y];
                for(int x = left ? 1 : 0; x < arena.size.x - (left ? 0 : 1); x++)
                {
                    for(int y = 0; y < arena.size.y; y++)
                    {
                        newArena[left ? x - 1 : x, y] = oldArena[x, y];
                    }
                }

                arena.Walls = newArena;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawGrid(ScriptableArena arena)
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            for(int y = arena.size.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for(int x = 0; x < arena.size.x; x++)
                {
                    if(x == arena.entrance.x && y == arena.entrance.y) GUI.backgroundColor = Color.green;
                    else if(x == arena.exit.x && y == arena.exit.y) GUI.backgroundColor = Color.red;
                    else GUI.backgroundColor = Color.white;

                    int i = arena.size.x * y + x;
                    arena.walls[i] = GUILayout.Toggle(arena.walls[i], "", toggleStyle);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif