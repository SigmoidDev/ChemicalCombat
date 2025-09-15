using Random = System.Random;
using UnityEngine;

namespace Sigmoid.Puzzles
{
	public class SlidingBlock : Puzzle
	{
        [Header("Puzzle Grid")]
        [SerializeField] private ScriptableBlockPuzzle[] puzzles;
        [SerializeField] private Transform gridCentre;
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private GameObject wallPrefab;

        [Header("Slot Parts")]
        [SerializeField] private Transform slotParent;
        [SerializeField] private GameObject leftPrefab;
        [SerializeField] private GameObject rightPrefab;
        [SerializeField] private GameObject topPrefab;
        [SerializeField] private GameObject bottomPrefab;

        [Header("Sliding Block")]
        [SerializeField] private Transform slidingBlock;
        [SerializeField] private Rigidbody2D slidingBody;
        [SerializeField] private SpriteRenderer slidingSprite;
        [SerializeField] private Sprite successfulSprite;

        private void Awake()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            walkthroughLayer = LayerMask.NameToLayer("Walkthrough");
        }

		public override void Initialise(int seed)
        {
            Random rng = new Random(seed);
            BlockPuzzle puzzle = puzzles[rng.Next(puzzles.Length)].puzzle;

            int rotations = rng.Next(4);
            for(int i = 0; i < rotations; i++)
                puzzle = puzzle.Rotate();

            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    int index = 7 * y + x;
                    if(puzzle.walls[index])
                    {
                        bool onEdge = x == 0 || x == 6 || y == 0 || y == 6;
                        GameObject block = Instantiate(onEdge ? wallPrefab : blockPrefab, gridCentre);
                        block.transform.localPosition = new Vector2(x - 3, y - 3);
                    }
                }
            }

            startPosition = puzzle.start - 3f * Vector2.one;
            if(puzzle.start.y == 6) startPosition.y += 0.125f;
            slidingBlock.localPosition = startPosition;

            targetPosition = puzzle.exit - 3f * Vector2.one;
            if(puzzle.exit.y == 6) targetPosition.y += 0.125f;

            PlaceSlot(puzzle.start);
            PlaceSlot(puzzle.exit);
        }

        private void PlaceSlot(Vector2Int position)
        {
            GameObject slot = Instantiate(position.x == 0 ? leftPrefab
                                        : position.x == 6 ? rightPrefab
                                        : position.y == 0 ? bottomPrefab
                                        : topPrefab, slotParent);
            
            slot.transform.localPosition =
              position.x == 0 ? new Vector2(-3.0625f, position.y - 3.0625f)
            : position.x == 6 ? new Vector2(3.0625f, position.y - 3.0625f)
            : position.y == 0 ? new Vector2(position.x - 3f, -3.125f)
            :                   new Vector2(position.x - 3f, 3.125f);
        }

        private Vector3 targetPosition;
        private Vector3 startPosition;

        private bool AtStart => Vector3.Distance(slidingBlock.localPosition, startPosition) <= 0.05f;
        private bool AtExit => Vector3.Distance(slidingBlock.localPosition, targetPosition) <= 0.05f;

        private bool completed;
        private bool DetectWins()
        {
            if(!AtExit) return false;

            slidingBody.isKinematic = true;
            slidingBody.velocity = Vector2.zero;
            slidingBlock.localPosition = targetPosition;

            slidingSprite.sprite = successfulSprite;
            completed = true;
            Succeed();

            return true;
        }

        private int defaultLayer;
        private int walkthroughLayer;

        /// <summary>
        /// Locks the sliding block to a constant velocity in the cardinal directions
        /// </summary>
        private void Update()
        {
            if(completed || DetectWins()) return;
            CheckOutOfBounds();

            float magnitude = slidingBody.velocity.magnitude;
            if(magnitude <= 0.04f)
            {
                slidingBlock.gameObject.layer = defaultLayer;
                if(!AtStart) slidingBlock.localPosition = new Vector2(
                    Mathf.Round(slidingBlock.localPosition.x),
                    Mathf.Round(slidingBlock.localPosition.y)
                );
                return;
            }

            float angle = Mathf.Atan2(slidingBody.velocity.y, slidingBody.velocity.x);
            angle = Mathf.Round(2f * angle / Mathf.PI) * 0.5f * Mathf.PI;

            slidingBlock.gameObject.layer = walkthroughLayer;
            slidingBody.velocity = 4f * new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );
        }

        private void CheckOutOfBounds()
        {
            if(slidingBlock.localPosition.x < -3.0625f
            || slidingBlock.localPosition.x > 3.0625f
            || slidingBlock.localPosition.y < -3.125f
            || slidingBlock.localPosition.y > 3.125f)
                slidingBody.velocity = Vector2.zero;

            slidingBlock.localPosition = new Vector2(
                Mathf.Clamp(slidingBlock.localPosition.x, -3.0625f, 3.0625f),
                Mathf.Clamp(slidingBlock.localPosition.y, -3.125f, 3.125f)
            );
        }
	}
}
