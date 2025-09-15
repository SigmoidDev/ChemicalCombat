using Sigmoid.Enemies;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Reactions
{
    public class CloudPuff : Explosion
    {
        protected override float EffectiveRadius => 2f;

        protected override void Effect(IAttackable attackable)
        {
            base.Effect(attackable);

            Vector2 outwardDirection = attackable.Position - (Vector2) transform.position;
            if((hitMask & HitMask.Enemies) != 0 && attackable is EnemyAttackable enemy)
            {
                enemy.Enemy.PermitForces(0.5f, 2f);
                enemy.Enemy.Body.AddForce(4f * outwardDirection.normalized, ForceMode2D.Impulse);
            }
            else if((hitMask & HitMask.Players) != 0 && attackable is PlayerAttackable player)
                player.Velocity += 20f * outwardDirection.normalized;
        }
    }
}
