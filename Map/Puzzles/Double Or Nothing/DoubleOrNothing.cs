using System.Collections;
using Random = System.Random;
using Sigmoid.Interactables;
using Sigmoid.Upgrading;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine;
using TMPro;

namespace Sigmoid.Puzzles
{
    public class DoubleOrNothing : Puzzle
    {
        private static WaitForSeconds _waitForSeconds0_75 = new WaitForSeconds(0.75f);
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ScriptableAudio coinSound;
        private CustomChest rewardChest;

        private int MinStarting => 2 * FloorManager.Instance.FloorNumber + 3;
        private int MaxStarting => 3 * FloorManager.Instance.FloorNumber + 6;
        private int BonusStarting => Perks.Has(Perk.Scavenger) ? 3 : 0;

        private Random rng;
        public int Value { get; private set; }
        public bool IsFlipping { get; private set; }
        public override void Initialise(int seed)
        {
            rng = new Random(seed);
            Value = rng.Next(MinStarting, MaxStarting) + BonusStarting;
            text.SetText(Value.ToString());
            rewardChest = (CustomChest) Reward;
            rewardChest.OnOpened += Open;
            rewardChest.OnOpened += () => Debug.Log(rewardChest);
            rewardChest.NumCoins = Value;
            rewardChest.MaxCoins = 2;
        }

        private int numFlips;
        public IEnumerator TakeChance()
        {
            IsFlipping = true;
            animator.Play("Flip");
            AudioPlayer.Play(coinSound, AudioChannel.Sound);
            yield return _waitForSeconds0_75;
            IsFlipping = false;

            float chance = 7 - numFlips;
            bool lucky = rng.Next(10) < chance;
            Value *= lucky ? 2 : 0;

            if(Value == 0)
            {
                Fail();
                rewardChest.Lock();
                text.SetText("FAIL");
                text.color = new Color(1.000f, 0.412f, 0.412f, 1.000f);
                yield break;
            }

            text.SetText(Value.ToString());
            rewardChest.NumCoins = Value;
            rewardChest.MaxCoins += 2;
            numFlips++;
        }

        public void Open()
        {
            Debug.Log("Opened");
            Succeed();
            Value = 0;
        }
    }
}
