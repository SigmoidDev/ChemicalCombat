using System.Collections.Generic;
using System.Collections;
using Random = System.Random;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class LightOrdering : Puzzle
    {
        private static WaitForSeconds _waitForSeconds0_2 = new WaitForSeconds(0.2f);
        [SerializeField] private List<LightTotem> totems;
        public LightState State { get; private set; }

        private Random rng;
        private int[] sequence;
        private int index;

        private int sequenceLength;
        private float flashDuration;
        private float flashInterval;

        public override void Initialise(int seed)
        {
            State = LightState.Waiting;
            rng = new Random(seed);

            SetDifficulty(DifficultyManager.Difficulty);
            sequence = new int[sequenceLength];
            index = 0;
        }

        public void HighlightAll()
        {
            foreach(LightTotem totem in totems)
                totem.HighlightMaterial();
        }

        public void UnhighlightAll()
        {
            foreach(LightTotem totem in totems)
                totem.UnhighlightMaterial();
        }

        /// <summary>
        /// Sets the length and difficulty of the sequence based on the chosen difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        private void SetDifficulty(Difficulty difficulty)
        {
            switch(difficulty)
            {
                case Difficulty.Rookie:
                {
                    sequenceLength = 4;
                    flashDuration = 0.50f;
                    flashInterval = 0.20f;
                    break;
                }
                case Difficulty.Skilled:
                {
                    sequenceLength = 5;
                    flashDuration = 0.45f;
                    flashInterval = 0.15f;
                    break;
                }
                case Difficulty.Veteran:
                {
                    sequenceLength = 6;
                    flashDuration = 0.40f;
                    flashInterval = 0.10f;
                    break;
                }
            }
        }

        public IEnumerator ShowSequence()
        {
            UnhighlightAll();
            State = LightState.Displaying;
            yield return _waitForSeconds0_2;

            for(int i = 0; i < sequenceLength; i++)
            {
                int next = rng.Next(5);
                sequence[i] = next;

                yield return totems[next].Flash(flashDuration);
                yield return new WaitForSeconds(flashInterval);
            }

            State = LightState.Guessing;
        }

        public IEnumerator Guess(int guess)
        {
            yield return totems[guess].Flash(0.2f);

            int correctIndex = sequence[index];
            if(guess == correctIndex)
            {
                if(index == sequenceLength - 1)
                {
                    foreach(LightTotem totem in totems)
                        totem.Light();

                    State = LightState.Completed;
                    Succeed();
                }
                index++;
            }
            else
            {
                foreach(LightTotem totem in totems)
                    totem.Crack();

                State = LightState.Completed;
                Fail();
            }
        }
    }

    public enum LightState
    {
        Waiting,
        Displaying,
        Guessing,
        Completed
    }
}
