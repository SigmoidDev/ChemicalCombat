using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class EnemyFlipper : SpriteFlipper
	{
		[SerializeField] private Enemy me;
        protected override Vector2 Velocity => me.Velocity;
        protected override bool Enabled => true;
    }
}
