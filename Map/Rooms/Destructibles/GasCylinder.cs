using Sigmoid.Reactions;

namespace Sigmoid.Rooms
{
    public class GasCylinder : DestructibleObject
    {
        public override string DisplayName => "Gas Cylinder";
        public override DetonatedPool ExplosionPool => ReactionPool.Instance.CloudPool;
    }
}
