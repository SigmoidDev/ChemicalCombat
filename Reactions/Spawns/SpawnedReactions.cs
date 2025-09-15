using Sigmoid.Projectiles;
using Sigmoid.Enemies;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Reactions
{
    public class AcidReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
    {
        public override SpawnedPool Pool => ReactionPool.Instance.AcidPool;
        public override float Cooldown => 1.0f;
    }

    public class CatalyseReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
	{
        public override SpawnedPool Pool => ReactionPool.Instance.CatalysePool;
        public override float Cooldown => 2.5f;
    }

    public class CrystalReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
	{
        public override SpawnedPool Pool => ReactionPool.Instance.CrystalPool;
        public override float Cooldown => 3.0f;
    }

    public class MagmaReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
    {
        public override SpawnedPool Pool => ReactionPool.Instance.MagmaPool;
        public override float Cooldown => 2.0f;
    }

    public class PlasmaReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
    {
        public override SpawnedPool Pool => ReactionPool.Instance.PlasmaPool;
        public override float Cooldown => 2.5f;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(cooldown > 0f) return;
			cooldown = Cooldown;

            PlasmaBall ball = (PlasmaBall) Pool.Fetch();
            ball.Initialise(Pool, hit.origin, Player.Instance, HitMask.Enemies | HitMask.Objects, damageMultiplier);

            Vector2 playerDelta = (Vector2) Player.Instance.transform.position - hit.point;
            ball.SetMoving(-playerDelta.normalized);
        }
    }

	public class StinkReaction : SpawnReaction<SpawnedPool, SpawnedEffect>
    {
        public override SpawnedPool Pool => ReactionPool.Instance.StinkPool;
        public override float Cooldown => 1.0f;
    }
}
