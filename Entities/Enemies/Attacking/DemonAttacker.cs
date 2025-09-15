using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class DemonAttacker : CompositeAttacker<DemonParams>
    {
        public DemonAttacker(Enemy enemy, DemonParams parameters) : base(enemy, parameters)
        {

        }

        public override void Initialise(){}


        public override void Update(IAttackable target, float deltaTime)
        {
            base.Update(target, deltaTime);
        }
    }
}
