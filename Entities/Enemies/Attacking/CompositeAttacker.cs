using System.Collections.Generic;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Allows for multiple different attacker types to be switched between by index
    /// </summary>
    public class CompositeAttacker<TParams> : AttackerBase<TParams> where TParams : CompositeAttackerParams
    {
        public CompositeAttacker(Enemy enemy, TParams parameters) : base(enemy, parameters)
        {
            attackers = new List<IAttacker>();
            foreach(AttackerParams subAttacker in parameters.Params)
                attackers.Add(subAttacker.CreateModule(enemy));
        }

        public override void Initialise(){}

        private readonly List<IAttacker> attackers;
        protected int currentIndex;
        public IAttacker CurrentAttacker => attackers[currentIndex];

        public IAttacker GetMode(int index) => attackers[index];
        public void SetMode(int index)
        {
            currentIndex = index;
            CurrentAttacker.Initialise();
        }

        public override void Update(IAttackable target, float deltaTime) => CurrentAttacker?.Update(target, deltaTime);
    }
}