using System;
using Sigmoid.Interactables;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    /// <summary>
    /// The base for any puzzle which allows it to hook in with a PuzzleRoom
    /// </summary>
	public abstract class Puzzle : MonoBehaviour
	{
        [field: SerializeField] public LootChest Reward { get; private set; }
        [field: SerializeField] public AudioPlayer AudioPlayer { get; private set; }
        [field: SerializeField] public ScriptableAudio SuccessAudio { get; private set; }
        [field: SerializeField] public ScriptableAudio FailureAudio { get; private set; }
		public event Action<bool> OnComplete;

        public abstract void Initialise(int seed);

        protected void Succeed()
        {
            OnComplete?.Invoke(true);
            AudioPlayer.Play(SuccessAudio, AudioChannel.Sound);
            Reward.Unlock();
        }

        protected void Fail()
        {
            OnComplete?.Invoke(false);
            AudioPlayer.Play(FailureAudio, AudioChannel.Sound);
        }
    }

    [Serializable]
    public class PuzzleObject
    {
        public PuzzleType type;
        public Puzzle prefab;
    }

    public enum PuzzleType
    {
        TicTacToe,
        SlidingBlock,
        LightOrdering,
        DoubleOrNothing,
        ReactionGame,
        MirrorPuzzle,
        CupShuffle
    }
}
