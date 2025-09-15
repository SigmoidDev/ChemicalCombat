namespace Sigmoid.Enemies
{
    /// <summary>
    /// Base class for any attack that is executed on a cooldown-timer basis
    /// </summary>
    public abstract class TimedAttacker<TParams> : AttackerBase<TParams> where TParams : TimedParams
    {
        public TimedAttacker(Enemy enemy, TParams parameters) : base(enemy, parameters)
        {
            onlyAttackNearby = parameters.OnlyAttackNearby;
            cooldown = parameters.AttackInterval;
            timer = 0f;
        }

        protected readonly bool onlyAttackNearby;
        protected readonly float cooldown;
        protected float timer;

        public override void Update(IAttackable target, float deltaTime)
        {
            timer -= deltaTime;
            if(timer > 0f || (!me.AtDestination && onlyAttackNearby)) return;

            timer = cooldown;
            Attack();
        }

        public abstract void Attack();
    }
}
