using System.Collections;
using Random = System.Random;
using Action = System.Action;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class ReactionGame : Puzzle
    {
        private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
        [SerializeField] private Animator standAnimator;
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject ellipsisObject;
        [SerializeField] private GameObject symbolObject;
        [SerializeField] private GameObject trophyObject;
        [SerializeField] private GameObject skullObject;
        [SerializeField] private SpriteRenderer symbolIcon;
        [SerializeField] private Sprite[] symbolSprites;

        public ReactionState State { get; private set; }

        private Random rng;
        public override void Initialise(int seed)
        {
            State = ReactionState.Inactive;
            rng = new Random(seed);

            numRounds = (int) DifficultyManager.Difficulty + 1;
            maxTime = DifficultyManager.Difficulty == Difficulty.Rookie ? 1.2f
                    : DifficultyManager.Difficulty == Difficulty.Skilled ? 0.9f : 0.7f;
        }

        private int numRounds;
        private float maxTime;

        private int clickedIndex;
        public void Click(int index) => clickedIndex = index;

        public IEnumerator StartGuessing()
        {
            State = ReactionState.Waiting;
            yield return RectractAndExtend(() =>
            {
                playButton.SetActive(false);
                ellipsisObject.SetActive(true);
            });

            for(int i = 0; i < numRounds; i++)
            {
                yield return new WaitForSeconds(0.01f * rng.Next(100, 400)); //1.0 - 4.0s
                State = ReactionState.Reacting;
                clickedIndex = -1;

                int correctValue = rng.Next(6);
                symbolIcon.sprite = symbolSprites[correctValue];
                ellipsisObject.SetActive(false);
                symbolObject.SetActive(true);

                float timeElapsed = 0f;
                while(timeElapsed < maxTime && clickedIndex == -1)
                    yield return timeElapsed += Time.deltaTime;

                bool failed = timeElapsed >= maxTime || clickedIndex != correctValue;
                bool won = i == numRounds - 1;

                //both winning and losing should stop the game
                State = !won && !failed ? ReactionState.Waiting : ReactionState.Completed;

                yield return RectractAndExtend(() =>
                {
                    symbolObject.SetActive(false);
                    GameObject toBeActivated = failed ? skullObject
                                             : won ? trophyObject
                                             : ellipsisObject;
                    toBeActivated.SetActive(true);
                });

                if(failed)
                {
                    Fail();
                    yield break;
                }
                else if(won)
                {
                    Succeed();
                    yield break;
                }
            }
        }

        private IEnumerator RectractAndExtend(Action middleAction)
        {
            standAnimator.Play("Retract");
            yield return _waitForSeconds0_5;

            middleAction?.Invoke();

            standAnimator.Play("Extend");
            yield return _waitForSeconds0_5;
        }
    }

    public enum ReactionState
    {
        Inactive,
        Waiting,
        Reacting,
        Completed
    }
}
