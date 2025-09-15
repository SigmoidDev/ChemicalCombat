using System.Collections.Generic;
using System;
using Sigmoid.Projectiles;
using Sigmoid.Enemies;
using Sigmoid.Players;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// A reaction which applies some buff to the player
    /// </summary>
	public abstract class BuffReaction : Reaction
    {
        public abstract float Duration { get; }
        protected abstract List<Guid> ApplyEffects();
        protected abstract List<Action<Guid>> RemovalFunctions { get; }
        private List<Guid> guids;

        private float timer;
        public override void Update(float deltaTime)
        {
            if(timer < 0f && guids != null)
            {
                for(int i = 0; i < guids.Count; i++)
                    RemovalFunctions[i]?.Invoke(guids[i]);

                guids = null;
            }
            else timer -= deltaTime;
        }

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            timer = Duration;
            guids ??= ApplyEffects();
        }
    }



    /// <summary>
    /// Boosts movement speed and fire rate by 30% for 4s
    /// </summary>
    public class CaffeineReaction : BuffReaction
    {
        public override float Duration => 4.0f;
        protected override List<Guid> ApplyEffects() => new List<Guid>
        {
            PlayerStats.MoveSpeed.AddModifier(speed => speed + 30f),
            PlayerStats.ThrowRate.AddModifier(rate => rate * 0.7f)
        };

        protected override List<Action<Guid>> RemovalFunctions => new List<Action<Guid>>
        {
            (id) => PlayerStats.MoveSpeed.RemoveModifier(id),
            (id) => PlayerStats.ThrowRate.RemoveModifier(id)
        };
    }

    /// <summary>
    /// Grants the midas buff, which drops a coin for every reaction triggered, for 10s
    /// </summary>
    public class MidasReaction : BuffReaction
    {
        public override float Duration => 10.0f;
        protected override List<Guid> ApplyEffects() => new List<Guid>
        {
            PlayerBuffs.Instance.GrantMidasBuff()
        };

        protected override List<Action<Guid>> RemovalFunctions => new List<Action<Guid>>
        {
            (id) => PlayerBuffs.Instance.RemoveMidasBuff()
        };
    }

    /// <summary>
    /// Increases dodge chance by 50% for 5s
    /// </summary>
    public class SteelReaction : BuffReaction
    {
        public override float Duration => 5.0f;
        protected override List<Guid> ApplyEffects() => new List<Guid>
        {
            PlayerStats.DodgeChance.AddModifier(chance => chance + 50f)
        };

        protected override List<Action<Guid>> RemovalFunctions => new List<Action<Guid>>
        {
            (id) => PlayerStats.DodgeChance.RemoveModifier(id)
        };
    }
}
