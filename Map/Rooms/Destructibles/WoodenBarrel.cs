using Sigmoid.Reactions;

namespace Sigmoid.Rooms
{
    public class WoodenBarrel : DestructibleObject
    {
        public override string DisplayName => "Wooden Barrel";
        public override DetonatedPool ExplosionPool => ReactionPool.Instance.FusionPool;
    }
}
