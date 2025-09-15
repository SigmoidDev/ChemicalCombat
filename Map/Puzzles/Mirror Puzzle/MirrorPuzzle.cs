using System.Collections.Generic;
using Random = System.Random;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    public class MirrorPuzzle : Puzzle
    {
        [SerializeField] private LineRenderer laser;
        [SerializeField] private Transform targetIndicator;
        private Vector2Int endPosition;

        [SerializeField] private Mirror mirrorPrefab;
        [SerializeField] private Transform mirrorParent;
        private Dictionary<Vector2Int, Mirror> activeMirrors;


        public bool IsCompleted { get; private set; }
        public override void Initialise(int seed)
        {
            int minMirrors = DifficultyManager.Difficulty == Difficulty.Rookie ? 8
                           : DifficultyManager.Difficulty == Difficulty.Skilled ? 10 : 12;

            Random rng = new Random(seed);
            (List<Vector2Int> mirrors, Vector2Int end) = MirrorGenerator.GenerateMirrorPuzzle(rng, minMirrors);
            while(mirrors == null) (mirrors, end) = MirrorGenerator.GenerateMirrorPuzzle(rng, minMirrors);

            activeMirrors = new Dictionary<Vector2Int, Mirror>();
            foreach(Vector2Int mirrorPosition in mirrors)
            {
                Mirror mirror = Instantiate(mirrorPrefab, mirrorParent);
                mirror.transform.localPosition = ToV3(mirrorPosition);
                if(rng.NextDouble() < 0.5) mirror.InteractWith();
                mirror.OnRotate += UpdateBeam;
                mirror.Initialise(this);

                activeMirrors.Add(mirrorPosition, mirror);
            }

            endPosition = end;
            targetIndicator.localPosition = new Vector2(2.90625f, 0.15625f + 0.5f * (endPosition.y - 3));

            while(UpdateBeam(true))
            {
                foreach(KeyValuePair<Vector2Int, Mirror> activeMirror in activeMirrors)
                    if(rng.NextDouble() < 0.5) activeMirror.Value.InteractWith();
            }
        }

        private void UpdateBeam() => UpdateBeam(false);
        private bool UpdateBeam(bool ignore = false)
        {
            Vector2Int currentPosition = new Vector2Int(0, 3);
            Vector2Int direction = Vector2Int.right;

            List<Vector3> points = new List<Vector3>{ ToV3(currentPosition) - 0.25f * Vector3.right };
            while(WithinBounds(currentPosition))
            {
                currentPosition += direction;
                if(activeMirrors.TryGetValue(currentPosition, out Mirror mirror))
                {
                    points.Add(ToV3(currentPosition));

                    direction =
                    direction == Vector2Int.right ? (mirror.IsFlipped ? Vector2Int.down : Vector2Int.up) :
                    direction == Vector2Int.left ? (mirror.IsFlipped ? Vector2Int.up : Vector2Int.down) :
                    direction == Vector2Int.up ? (mirror.IsFlipped ? Vector2Int.left : Vector2Int.right) :
                    direction == Vector2Int.down ? (mirror.IsFlipped ? Vector2Int.right : Vector2Int.left) : direction;
                }
            }

            if(currentPosition == endPosition)
            {
                if(ignore) return true;

                Succeed();
                IsCompleted = true;
                laser.startColor = new Color(0.063f, 0.961f, 0.271f, 1.000f);
                laser.endColor = new Color(0.459f, 0.910f, 0.090f, 1.000f);
            }
            points.Add(ToV3(currentPosition) - 0.25f * (Vector3)(Vector2) direction);

            laser.positionCount = points.Count;
            laser.SetPositions(points.ToArray());
            return false;
        }

        private Vector3 ToV3(Vector2Int v2int) => 0.5f * new Vector3(v2int.x - 5, v2int.y - 3, 0);
        private bool WithinBounds(Vector2Int position) => position.x >= 0 && position.x <= 10 && position.y >= 0 && position.y <= 6;
    }
}
