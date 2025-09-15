using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Spirit Params", menuName = "Enemies/Attacking/Spirit")]
    public class SpiritParams : AttackerParams
    {
		[field: SerializeField] public float MaxRange { get; private set; }
		[field: SerializeField] public float ForceCoefficient { get; private set; }
		[field: SerializeField] public Color[] OriginalColours { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new SpiritAttacker(enemy, this);
    }
}