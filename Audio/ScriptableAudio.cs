using UnityEngine;

namespace Sigmoid.Audio
{
    [CreateAssetMenu(fileName = "New Audio", menuName = "Create New Audio")]
	public class ScriptableAudio : ScriptableObject
	{
		public AudioClip[] audioClips;
        public AudioClip Clip => audioClips[Random.Range(0, audioClips.Length)];

        public Vector2 volumeRange = Vector2.one;
        public Vector2 pitchRange = Vector2.one;

        [Space]
        public bool loop;
        public bool is3D;
	}
}
