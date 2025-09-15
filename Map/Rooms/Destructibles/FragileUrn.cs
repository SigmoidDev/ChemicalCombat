using Sigmoid.Reactions;

namespace Sigmoid.Rooms
{
    public class FragileUrn : DestructibleObject
    {
        public override string DisplayName => "Fragile Urn";
        public override DetonatedPool ExplosionPool => ReactionPool.Instance.FusionPool;
    }
}
