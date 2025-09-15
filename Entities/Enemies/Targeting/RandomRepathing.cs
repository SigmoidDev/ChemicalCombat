using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Random Repathing", menuName = "Enemies/Targeting/Repathing/Random")]
    public class RandomRepathing : ScriptableInterval
    {
        [SerializeField] private Vector2 intervalRange;
        public override RepathInterval Create() => new RandomRepather(intervalRange.x, intervalRange.y);
    }

    /// <summary>
    /// Repaths on a timer, but with a range instead of a fixed value
    /// </summary>
    public class RandomRepather : RepathInterval
    {
        private readonly float minInterval;
        private readonly float maxInterval;
        private float timer;

        public RandomRepather(float minInterval, float maxInterval)
        {
            this.minInterval = minInterval;
            this.maxInterval = maxInterval;
            timer = 0f;
        }

        public override bool ShouldRepath => timer <= 0f;
        public override void Reset() => timer = Random.Range(minInterval, maxInterval);
        public override void Update() => timer -= Time.deltaTime;
    }
}
