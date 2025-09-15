using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Simply pathfinds from A to B, provided that the position is on the NavMesh
    /// </summary>
	public class DirectMovement : MovementBase<DirectParams>
	{
        public DirectMovement(Enemy enemy, DirectParams parameters) : base(enemy, parameters){}

        public override void Initialise()
        {
            me.SpeedMultiplier.OnStatChanged += UpdateSpeed;
            UpdateBaseSpeed(parameters.Speed);
            UpdateSpeed(me.SpeedMultiplier);
        }
        public override void Destroy() => me.SpeedMultiplier.OnStatChanged -= UpdateSpeed;

        private float baseSpeed;
        public void UpdateBaseSpeed(float newSpeed)
        {
            baseSpeed = newSpeed;
            UpdateSpeed(me.SpeedMultiplier);
        }

        public void UpdateSpeed(float speedMult)
        {
            me.Agent.speed = baseSpeed * speedMult * DifficultyManager.SpeedMultipler;
            me.Agent.enabled = me.Agent.speed != 0f;
        }

        public override void Update(Vector2 targetPosition, float deltaTime)
        {
            if(me.Agent.enabled && me.Agent.isOnNavMesh)
                me.Agent.SetDestination(targetPosition);
        }
	}
}
