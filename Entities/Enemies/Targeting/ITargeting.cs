using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// The AI module that controls where an enemy should move to based on its target
    /// </summary>    
    public interface ITargeting
    {
        public bool GetDestination(IAttackable target, out Vector2 destination);
        public void Initialise();
        public void Destroy();
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Base class for determining where an enemy should move to relative to their target
    /// </summary>
	public abstract class TargetingBase<TParams> : ITargeting where TParams : TargetingParams
    {
        protected readonly Enemy me;
        protected readonly TParams parameters;
        public TargetingBase(Enemy enemy, TParams parameters)
        {
            me = enemy;
            this.parameters = parameters;
            IsEnabled = true;
        }

        public virtual void Initialise(){}
        public virtual void Destroy(){}
        public bool IsEnabled { get; set; }

        public abstract bool GetDestination(IAttackable target, out Vector2 destination);
    }

    /// <summary>
    /// Stores any required properties for a TargetingBase
    /// </summary>
    public abstract class TargetingParams : ScriptableObject
    {
        public abstract ITargeting CreateModule(Enemy enemy);
    }



    /// <summary>
    /// Represents some way of knowing how often to repath
    /// </summary>
    public abstract class ScriptableInterval : ScriptableObject
    {
        public abstract RepathInterval Create();
    }

    /// <summary>
    /// Determines how often a given Targeting should repath
    /// </summary>
    public abstract class RepathInterval
	{
		public abstract bool ShouldRepath { get; }
        public abstract void Update();
        public abstract void Reset();
	}
}
