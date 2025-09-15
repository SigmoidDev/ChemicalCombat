using Sigmoid.Reactions;

namespace Sigmoid.Rooms
{
    public class ManaJar : DestructibleObject
    {
        public override string DisplayName => "Mana Jar";
        public override DetonatedPool ExplosionPool => ReactionPool.Instance.FireworkPool;
    }
}
