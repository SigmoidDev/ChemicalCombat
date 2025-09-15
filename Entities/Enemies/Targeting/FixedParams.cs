using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Fixed")]
	public class FixedParams : TargetingParams
	{
        [field: SerializeField] public PathType Type { get; private set; }
        [field: SerializeField] public float PointAccuracy { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new FixedTargeting(enemy, this);
	}

    public enum PathType
    {
        Corners,
        Horizontal,
        Vertical,
        Rows,
        Columns
    }
}
