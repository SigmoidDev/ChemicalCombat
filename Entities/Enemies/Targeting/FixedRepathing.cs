using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Fixed Repathing", menuName = "Enemies/Targeting/Repathing/Fixed")]
    public class FixedRepathing : ScriptableInterval
    {
        [SerializeField] private float interval;
        public override RepathInterval Create() => new FixedRepather(interval);
    }

    /// <summary>
    /// Repaths every fixed interval
    /// </summary>
    public class FixedRepather : RepathInterval
    {
        private readonly float interval;
        private float timer;

        public FixedRepather(float interval)
        {
            this.interval = interval;
            timer = 0f;
        }

        public override bool ShouldRepath => timer <= 0f;
        public override void Reset() => timer = interval;
        public override void Update() => timer -= Time.deltaTime;
    }
}
