#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;

namespace Sigmoid.Utilities
{
    [ExecuteInEditMode]
    public class NavigationVisualiser : MonoBehaviour
    {
        [SerializeField] private Color fillColour = new Color(0f, 0.5f, 1f, 0.25f);
        [SerializeField] private Color lineColour = new Color(0f, 0.5f, 1f, 0.25f);

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(Selection.activeObject != gameObject) return;

            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = fillColour;
            Gizmos.color = lineColour;

            for (int i = 0; i < triangulation.indices.Length; i += 3)
            {
                Vector3 p1 = triangulation.vertices[triangulation.indices[i]];
                Vector3 p2 = triangulation.vertices[triangulation.indices[i + 1]];
                Vector3 p3 = triangulation.vertices[triangulation.indices[i + 2]];

                p1.z = 0;
                p2.z = 0;
                p3.z = 0;

                Handles.DrawAAConvexPolygon(p1, p2, p3);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p1);
            }
        }
        #endif
    }
}