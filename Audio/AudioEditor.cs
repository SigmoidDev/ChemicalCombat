#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sigmoid.Audio
{
    [CustomEditor(typeof(ScriptableAudio)), CanEditMultipleObjects]
	public class AudioEditor : Editor
	{
        private SerializedProperty audioClips;
        private SerializedProperty volumeRange;
        private SerializedProperty pitchRange;
        private SerializedProperty loop;
        private SerializedProperty is3D;

        private void OnEnable()
        {
            audioClips = serializedObject.FindProperty("audioClips");
            volumeRange = serializedObject.FindProperty("volumeRange");
            pitchRange = serializedObject.FindProperty("pitchRange");
            loop = serializedObject.FindProperty("loop");
            is3D = serializedObject.FindProperty("is3D");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(audioClips);

            float minVolume = volumeRange.vector2Value.x;
            float maxVolume = volumeRange.vector2Value.y;
            string volumeLabel = minVolume == maxVolume ? FormatPercentage(minVolume)
            : $"{FormatPercentage(minVolume)}-{FormatPercentage(maxVolume)}";

            EditorGUILayout.MinMaxSlider($"Volume ({volumeLabel}) ", ref minVolume, ref maxVolume, 0f, 2f);
            volumeRange.vector2Value = new Vector2(
                0.05f * Mathf.Round(minVolume * 20f),
                0.05f * Mathf.Round(maxVolume * 20f)
            );

            float minPitch = pitchRange.vector2Value.x;
            float maxPitch = pitchRange.vector2Value.y;
            string pitchLabel = minPitch == maxPitch ? FormatPercentage(minPitch)
            : $"{FormatPercentage(minPitch)}-{FormatPercentage(maxPitch)}";

            EditorGUILayout.MinMaxSlider($"Pitch ({pitchLabel}) ", ref minPitch, ref maxPitch, 0f, 2f);
            pitchRange.vector2Value = new Vector2(
                0.05f * Mathf.Round(minPitch * 20f),
                0.05f * Mathf.Round(maxPitch * 20f)
            );

            EditorGUILayout.PropertyField(loop);
            EditorGUILayout.PropertyField(is3D);
            serializedObject.ApplyModifiedProperties();
        }

        private string FormatPercentage(float value) => value * 100f + "%";
    }
}
#endif