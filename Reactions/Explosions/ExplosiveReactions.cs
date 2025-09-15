namespace Sigmoid.Reactions
{
    public class FireworksReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.FireworkPool;
        public override float Cooldown => 3.5f;
    }

	public class FlashReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.FlashPool;
        public override float Cooldown => 0.1f;
    }

	public class FusionReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.FusionPool;
        public override float Cooldown => 0.8f;
    }

	public class ThermiteReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.ThermitePool;
        public override float Cooldown => 2.0f;
    }

	public class GunpowderReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.GunpowderPool;
        public override float Cooldown => 0.1f;
    }

	public class NuclearReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
	{
        public override DetonatedPool Pool => ReactionPool.Instance.NuclearPool;
        public override float Cooldown => 5.0f;
    }
}
