using Sigmoid.Utilities;
using Sigmoid.Game;
using UnityEngine.Audio;
using UnityEngine;
using System;

namespace Sigmoid.Audio
{
	public class AudioManager : Singleton<AudioManager>
	{
        [field: SerializeField] public AudioMixer Mixer { get; private set; }
        [field: SerializeField] public AudioMixerGroup MusicGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup SoundGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup EnemyGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup UIGroup { get; private set; }

        [SerializeField] private AudioPool pool;

        /// <summary>
        /// Plays a given sound at some location in the world, intended for sounds that would otherwise be deleted as part of their parent object
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="position"></param>
        /// <param name="channel"></param>
        public void Play(ScriptableAudio audio, Vector2 position, AudioChannel channel)
        {
            AudioPlayer player = pool.Fetch();
            player.transform.position = position;

            Action<AudioPlayer> action = audio.loop ? null : KillSound;
            player.Play(audio, channel, action);
        }

        /// <summary>
        /// Gets rid of an instantiated AudioPlayer
        /// </summary>
        /// <param name="player"></param>
        private void KillSound(AudioPlayer player) => pool.Release(player);

        private void Awake()
        {
            SceneLoader.Instance.OnSceneLoaded += GetMusic; //never unsubscribed
            GetMusic(SceneLoader.Instance.CurrentScene);
        }

        [SerializeField] private ScriptableAudio menuTrack;
        [SerializeField] private ScriptableAudio homeTrack;
        [SerializeField] private MusicPlayer musicPlayer;

        /// <summary>
        /// Chooses which soundtrack to play based on the current scene and/or floor
        /// </summary>
        /// <param name="scene"></param>
        private void GetMusic(GameScene scene) => musicPlayer.Play(scene == GameScene.Labyrinth ? FloorManager.Instance.Floor.musicTrack
                                                                 : scene == GameScene.Menu ? menuTrack : homeTrack, AudioChannel.Music);

        /// <summary>
        /// Fades the music volume to 0 over 0.3s (the time taken to unload the scene)
        /// </summary>
        public void FadeOut() => musicPlayer.Fade(0f, 0.29f, musicPlayer.Stop);
	}

    /// <summary>
    /// Contains helper functions for audio-related things, such as output channels
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Sets the AudioMixerGroup of this AudioSource according to the AudioChannel provided
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channel"></param>
        public static void SetChannel(this AudioSource source, AudioChannel channel)
        {
            source.outputAudioMixerGroup = channel switch
            {
                AudioChannel.Music => AudioManager.Instance.MusicGroup,
                AudioChannel.Sound => AudioManager.Instance.SoundGroup,
                AudioChannel.Enemy => AudioManager.Instance.EnemyGroup,
                AudioChannel.UI => AudioManager.Instance.UIGroup,
                _ => null
            };
        }

        /// <summary>
        /// Converts a linear value (0% - 100%) to a logarithmic one (-80dB to 0dB)
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static float PercentageToDecibels(float percentage) => 20f * Mathf.Log10(Mathf.Max(0.0001f, 0.01f * percentage));
    }

    /// <summary>
    /// Determines which Audio Mixer a sound should come through
    /// </summary>
    public enum AudioChannel
    {
        Music,
        Sound,
        Enemy,
        UI
    }
}
