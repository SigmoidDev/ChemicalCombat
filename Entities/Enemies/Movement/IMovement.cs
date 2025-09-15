using UnityEngine;

namespace Sigmoid.Enemies
{
    public interface IMovement
    {
        public void Update(Vector2 targetPosition, float deltaTime);
        public void Initialise();
        public void Destroy();
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Base class for any method of moving to a given destination
    /// </summary>
	public abstract class MovementBase<TParams> : IMovement where TParams : MovementParams
    {
        protected readonly Enemy me;
        protected readonly TParams parameters;
        public MovementBase(Enemy enemy, TParams parameters)
        {
            me = enemy;
            this.parameters = parameters;
            IsEnabled = true;
        }

        public virtual void Initialise(){}
        public virtual void Destroy(){}
        public bool IsEnabled { get; set; }

        public abstract void Update(Vector2 targetPosition, float deltaTime);
    }

    /// <summary>
    /// Stores any required properties for a MovementBase
    /// </summary>
    public abstract class MovementParams : ScriptableObject
    {
        public abstract IMovement CreateModule(Enemy enemy);
    }



    public enum AccelerationType
    {
        Slow,
        Normal,
        Instant
    }

    public static class AccelerationHelper
    {
        public static float GetAccelerationValue(this AccelerationType type) => type switch
        {
            AccelerationType.Slow => 5f,
            AccelerationType.Normal => 40f,
            AccelerationType.Instant => 10000f,
            _ => 0
        };

        public static float GetTurningValue(this AccelerationType type) => type switch
        {
            AccelerationType.Slow => 30f,
            AccelerationType.Normal => 360f,
            AccelerationType.Instant => 3600f,
            _ => 0
        };
    }
}
