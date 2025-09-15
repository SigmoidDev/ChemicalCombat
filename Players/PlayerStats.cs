using System.Collections.Generic;
using System;
using Sigmoid.Utilities;

namespace Sigmoid.Players
{
    /// <summary>
    /// Holds the collection of various stats that affect the player's actions
    /// </summary>
	public class PlayerStats : Singleton<PlayerStats>
	{
        public static Stat<int> MaxHealth;
        public static Stat<float> DodgeChance;
        public static Stat<float> MoveSpeed;
        public static Stat<float> ImmunityPeriod;

        public static Stat<int> BaseDamage;
        public static Stat<float> ThrowRate;
        public static Stat<float> ReloadSpeed;
        public static Stat<float> SplashRadius;
        public static Stat<float> CritChance;

        public static Stat<float> ExplosionRadius;
        public static Stat<float> DoTRate;
        public static Stat<float> DebuffDuration;
        public static Stat<float> EffectDuration;

        public static Stat<int> PotionSize;
        public static Stat<int> LightLevel; 
        public static Stat<float> PickupRadius;
        public static Stat<float> ObjectDamage;
        public static Stat<int> BonusCoins;

        private void Awake()
        {
            MaxHealth = new Stat<int>(6);
            DodgeChance = new Stat<float>(0f);
            MoveSpeed = new Stat<float>(100f);
            ImmunityPeriod = new Stat<float>(1f);

            BaseDamage = new Stat<int>(2);
            ThrowRate = new Stat<float>(1f);
            ReloadSpeed = new Stat<float>(1f);
            SplashRadius = new Stat<float>(2f);
            CritChance = new Stat<float>(0f);

            ExplosionRadius = new Stat<float>(1f);
            DoTRate = new Stat<float>(1f);
            DebuffDuration = new Stat<float>(1f);
            EffectDuration = new Stat<float>(1f);

            PotionSize = new Stat<int>(3);
            LightLevel = new Stat<int>(1);
            PickupRadius = new Stat<float>(1f);
            ObjectDamage = new Stat<float>(1f);
            BonusCoins = new Stat<int>(0);
        }
	}

    /// <summary>
    /// Represents a Stat of any standard type that can be modified in any way
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Stat<T> where T : struct
    {
        private T cachedValue;
        private readonly T baseValue;
        private readonly List<StatModifier> modifiers;

        public struct StatModifier
        {
            public Guid id;
            public Func<T, T> apply;

            public StatModifier(Func<T, T> apply)
            {
                id = Guid.NewGuid();
                this.apply = apply;
            }
        }

        public Stat(T baseValue)
        {
            this.baseValue = baseValue;
            cachedValue = baseValue;
            modifiers = new List<StatModifier>();
        }

        public T Value => cachedValue;
        public static implicit operator T(Stat<T> stat) => stat.cachedValue;
        public event Action<T> OnStatChanged;

        /// <summary>
        /// Recalculates the value cached by looping through all modifiers
        /// </summary>
        public void RecalculateValue()
        {
            T oldValue = cachedValue;

            cachedValue = baseValue;
            foreach(StatModifier modifier in modifiers)
                cachedValue = modifier.apply(cachedValue);

            if(!cachedValue.Equals(oldValue))
                OnStatChanged?.Invoke(cachedValue);
        }

        /// <summary>
        /// Adds a modifier function to the stat and returns the Guid required to remove it if needed
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public Guid AddModifier(Func<T, T> effect)
        {
            StatModifier modifier = new StatModifier(effect);
            modifiers.Add(modifier);
            RecalculateValue();
            return modifier.id;
        }

        /// <summary>
        /// Attempts to remove the modifier with the given Guid and recalculates the stat's value
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Guid.Empty</returns>
        public Guid RemoveModifier(Guid id)
        {
            modifiers.RemoveAll(modifier => modifier.id == id);
            RecalculateValue();
            return Guid.Empty;
        }
    }
}
