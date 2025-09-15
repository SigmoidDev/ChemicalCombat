using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Effects
{
    public class PlayerFlipper : SpriteFlipper
    {
        protected override Vector2 Velocity => Player.Instance.Movement.Body.velocity;
        protected override bool Enabled => true;
    }
}
