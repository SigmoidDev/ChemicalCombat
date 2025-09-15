using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Movement/Phase")]
	public class PhaseParams : DirectParams
	{
        public override IMovement CreateModule(Enemy enemy) => new PhaseMovement(enemy, this);
    }
}
