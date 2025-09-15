using System.Collections;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class CupShuffle : Puzzle
    {
        private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
        private static WaitForSeconds _waitForSeconds1_6 = new WaitForSeconds(1.6f);
        [SerializeField] private ShufflableCup[] cups;
        [SerializeField] private Vector2[] ballPositions;
        [SerializeField] private GameObject ball;

        public ShuffleState State { get; private set; }
        private float animationSpeed;
        private float shuffleInterval;
        private int numShuffles;

        public override void Initialise(int seed)
        {
            State = ShuffleState.Inactive;
            cups[0].SetIndex(1);
            cups[1].SetIndex(2);
            cups[2].SetIndex(3);

            SetDifficulty(DifficultyManager.Difficulty);

            int ballIndex = Random.Range(0, 3);
            cups[ballIndex].HasBall = true;
            ball.transform.localPosition = ballPositions[ballIndex];
        }

        public void HighlightAll()
        {
            foreach(ShufflableCup cup in cups)
                cup.HighlightMaterial();
        }

        public void UnhighlightAll()
        {
            foreach(ShufflableCup cup in cups)
                cup.UnhighlightMaterial();
        }

        /// <summary>
        /// Adjusts the animation speed and number of shuffles based on the game difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        private void SetDifficulty(Difficulty difficulty)
        {
            switch(difficulty)
            {
                case Difficulty.Rookie:
                {
                    animationSpeed = 0.8f;
                    shuffleInterval = 0.75f;
                    numShuffles = 8;
                    break;
                }
                case Difficulty.Skilled:
                {
                    animationSpeed = 1f;
                    shuffleInterval = 0.60f;
                    numShuffles = 12;
                    break;
                }
                case Difficulty.Veteran:
                {
                    animationSpeed = 1.5f;
                    shuffleInterval = 0.35f;
                    numShuffles = 16;
                    break;
                }
            }
        }

        /// <summary>
        /// Randomly swaps the cups around a certain number of times
        /// </summary>
        /// <returns></returns>
        public IEnumerator Shuffle()
        {
            UnhighlightAll();
            State = ShuffleState.Shuffling;
            foreach(ShufflableCup cup in cups)
            {
                cup.SetSpeed(1f);
                cup.Lift();
            }

            yield return _waitForSeconds1_6;
            ball.SetActive(false);

            foreach(ShufflableCup cup in cups)
                cup.SetSpeed(animationSpeed);

            for(int i = 0; i < numShuffles; i++)
            {
                int notSelected = Random.Range(0, 3);
                if(notSelected == 0) Swap(1, 2);
                if(notSelected == 1) Swap(0, 2);
                if(notSelected == 2) Swap(0, 1);
                yield return new WaitForSeconds(shuffleInterval);
            }

            foreach(ShufflableCup cup in cups)
                cup.SetSpeed(1f);

            int finalIndex = cups[0].HasBall ? 0 : cups[1].HasBall ? 1 : 2;
            ball.transform.localPosition = ballPositions[finalIndex];
            ball.SetActive(true);

            State = ShuffleState.Choosing;
        }

        /// <summary>
        /// Swaps two cups with each other
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void Swap(int a, int b)
        {
            cups[a].SetIndex(b + 1);
            cups[b].SetIndex(a + 1);
            (cups[b], cups[a]) = (cups[a], cups[b]);
        }

        /// <summary>
        /// Finishes the game and reveals to the player whether or not they chose the right one
        /// </summary>
        /// <param name="cup"></param>
        /// <returns></returns>
        public IEnumerator ChooseCup(ShufflableCup cup)
        {
            State = ShuffleState.Completed;
            cup.Lift(true);

            yield return _waitForSeconds1;

            if(cup.HasBall) Succeed();
            else Fail();

            yield return new WaitForSeconds(0.5f);
            foreach(ShufflableCup other in cups)
            {
                if(other == cup) continue;
                other.Lift(true);
            }
        }
    }

    public enum ShuffleState
    {
        Inactive,
        Shuffling,
        Choosing,
        Completed
    }
}
