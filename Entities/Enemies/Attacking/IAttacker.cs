using System.Collections;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public interface IAttacker
    {
        public void Update(IAttackable target, float deltaTime);
        public void Initialise();
        public void Destroy();
        public bool IsEnabled { get; set; }
        public virtual IEnumerator Kill() => null;
    }

    /// <summary>
    /// Base class for any attacking or custom enemy logic to be implemented in
    /// </summary>
	public abstract class AttackerBase<TParams> : IAttacker where TParams : AttackerParams
    {
        protected readonly Enemy me;
        protected readonly TParams parameters;
        public AttackerBase(Enemy enemy, TParams parameters)
        {
            me = enemy;
            this.parameters = parameters;
            IsEnabled = true;
        }

        public virtual void Initialise(){}
        public virtual void Destroy(){}
        public bool IsEnabled { get; set; }

        public abstract void Update(IAttackable target, float deltaTime);
        public virtual IEnumerator Kill() => null;
    }

    /// <summary>
    /// Stores any required properties for an AttackerBase
    /// </summary>
    public abstract class AttackerParams : ScriptableObject
    {
        public abstract IAttacker CreateModule(Enemy enemy);
    }
}
