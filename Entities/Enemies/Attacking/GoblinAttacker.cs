using Sigmoid.Players;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Allows goblins to deal double damage when backstabbing
    /// </summary>
    public class GoblinAttacker : MeleeAttacker
    {
        public GoblinAttacker(Enemy enemy, GoblinParams parameters) : base(enemy, parameters){}

        protected override float GetDamage(IAttackable attackable)
        {
            if(attackable is PlayerAttackable)
            {
                bool playerFacingLeft = Player.Instance.Sprite.flipX;
                bool meFacingLeft = me.Sprite.flipX;

                bool onRight = me.transform.position.x >= attackable.Position.x;
                bool wasBackstab = (meFacingLeft  && onRight  && playerFacingLeft)
                                || (!meFacingLeft && !onRight && !playerFacingLeft);

                return wasBackstab ? 2f : 1f;
            }
            else return 8f;
        }
    }
}
