using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Composite Params", menuName = "Enemies/Attacking/Composite")]
    public abstract class CompositeAttackerParams : AttackerParams
    {
        [field: SerializeField] public List<AttackerParams> Params { get; private set; }
    }
}
