using Action = System.Action<Sigmoid.Audio.AudioPlayer>;
using System.Collections;
using UnityEngine;

namespace Sigmoid.Audio
{
    /// <summary>
    /// Allows for easy, dynamic setup of an AudioSource based on a ScriptableAudio object
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour
	{
		[SerializeField] protected AudioSource source;
        [SerializeField] protected ScriptableAudio audioClip;
        [SerializeField] protected AudioChannel audioChannel;
        public bool IsPlaying => source.isPlaying;

        /// <summary>
        /// Sets up the AudioSource's parameters based on a ScriptableAudio before playing it
        /// </summary>
        /// <param name="clip"></param>
        public void Play(ScriptableAudio clip, AudioChannel channel, Action OnComplete = null)
        {
            if(clip == null) return;
            audioClip = clip;
            audioChannel = channel;

            source.clip = clip.Clip;
            source.SetChannel(channel);
            source.volume = 0.5f * Random.Range(clip.volumeRange.x, clip.volumeRange.y);
            source.pitch = Random.Range(clip.pitchRange.x, clip.pitchRange.y);
            source.loop = clip.loop;
            source.spatialBlend = clip.is3D ? 0.7f : 0f;

            source.Play();
            if(OnComplete == null) return;

            if(activeCompletion != null) StopCoroutine(activeCompletion);
            activeCompletion = StartCoroutine(CompleteAfterPlaying(OnComplete));
        }
        public void Play() => Play(audioClip, audioChannel);

        /// <summary>
        /// Instantly stops the currently playing audio clip
        /// </summary>
        public void Stop() => source.Stop();

        private Coroutine activeCompletion;
        private IEnumerator CompleteAfterPlaying(Action OnComplete)
        {
            yield return new WaitWhile(() => source.isPlaying);
            OnComplete?.Invoke(this);
        }
    }
}
