using Sigmoid.Reactions;

namespace Sigmoid.Rooms
{
    public class NuclearBarrel : DestructibleObject
    {
        public override string DisplayName => "Nuclear Barrel";
        public override DetonatedPool ExplosionPool => ReactionPool.Instance.NuclearPool;
    }
}
