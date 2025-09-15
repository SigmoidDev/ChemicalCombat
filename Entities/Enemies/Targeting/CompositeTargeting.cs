using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Allows for switching between multiple Targetings by index
    /// </summary>
    public class CompositeTargeting : TargetingBase<CompositeTargetingParams>
    {
        public CompositeTargeting(Enemy enemy, CompositeTargetingParams parameters) : base(enemy, parameters)
        {
            targetings = new List<ITargeting>();
            foreach(TargetingParams subTargeting in parameters.Params)
                targetings.Add(subTargeting.CreateModule(enemy));
        }

        private readonly List<ITargeting> targetings;
        protected int currentIndex;
        public ITargeting CurrentTargeting => targetings[currentIndex];

        public ITargeting GetMode(int index) => targetings[index];
        public void SetMode(int index) => currentIndex = index;

        public override bool GetDestination(IAttackable target, out Vector2 destination) => CurrentTargeting.GetDestination(target, out destination);
    }
}
