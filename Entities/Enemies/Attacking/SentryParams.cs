using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Sentry Params", menuName = "Enemies/Attacking/Sentry")]
	public class SentryParams : ProjectileParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new SentryAttacker(enemy, this);
	}
}
