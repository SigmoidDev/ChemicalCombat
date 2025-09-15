using Sigmoid.Projectiles;
using Sigmoid.Enemies;
using Sigmoid.Buffs;

namespace Sigmoid.Reactions
{
    public class BleachReaction : Reaction
	{
        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            source.Damageable.DotReceiver.InflictDot(DotType.Burning, 2.5f);
            source.Damageable.DotReceiver.InflictDot(DotType.Dissolving, 2.5f);
            source.Damageable.DotReceiver.InflictDot(DotType.Poisoned, 2.5f);
        }
    }

    public class CorrosionReaction : Reaction
    {
        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier) => source.Damageable.DotReceiver.InflictDot(DotType.Corroding, 6f, 2);
    }

    public class CyanideReaction : Reaction
	{
        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier) => source.Damageable.DotReceiver.InflictDot(DotType.Poisoned, 2.5f, 3);
    }

	public class PoisonReaction : Reaction
	{
        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier) => source.Damageable.DotReceiver.InflictDot(DotType.Poisoned, 5f, 2);
    }
}
