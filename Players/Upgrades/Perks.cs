using System.Collections.Generic;
using System;
using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Upgrading
{
	public class Perks : Singleton<Perks>
	{
        [SerializeField] private List<ScriptablePerk> perkList;
        private Dictionary<Perk, ScriptablePerk> perkDict;
        public static ScriptablePerk GetInfo(Perk perk) => Instance.perkDict.TryGetValue(perk, out ScriptablePerk info) ? info : null;

        private int numUnlocked;
        private HashSet<Perk> unlockedPerks;
        private void Awake()
        {
            numUnlocked = 0;
            unlockedPerks = new HashSet<Perk>();
            perkDict = new Dictionary<Perk, ScriptablePerk>();
            foreach(ScriptablePerk perk in perkList)
                perkDict.Add(perk.associatedPerk, perk);
        }

        public event Action<Perk> OnPerkUnlocked;

        public static int NumUnlockedPerks => Instance.numUnlocked;
        public static bool Has(Perk perk) => Instance.unlockedPerks.Contains(perk);
        public static void Unlock(Perk perk)
        {
            Instance.numUnlocked++;
            Instance.OnPerkUnlocked?.Invoke(perk);
            Instance.unlockedPerks.Add(perk);
            Apply(perk);
        }

        public static void Apply(Perk perk)
        {
            switch(perk)
            {
                case Perk.Serrated:
                {
                    PlayerStats.BaseDamage.AddModifier(damage => damage * 2);
                    break;
                }
                case Perk.Afflictive:
                {
                    PlayerStats.DebuffDuration.AddModifier(duration => duration * 2f);
                    break;
                }
                case Perk.Effectual:
                {
                    PlayerStats.EffectDuration.AddModifier(duration => duration + 3f);
                    break;
                }
                case Perk.Pungent:
                {
                    PlayerStats.DoTRate.AddModifier(rate => rate * 1.5f);
                    break;
                }
                case Perk.Incendiary:
                {
                    PlayerStats.ExplosionRadius.AddModifier(radius => radius * 1.5f);
                    break;
                }
                case Perk.Superior:
                {
                    PlayerStats.CritChance.AddModifier(chance => chance + 25f);
                    break;
                }
                case Perk.Dispersed:
                {
                    PlayerStats.SplashRadius.AddModifier(radius => radius * 1.5f);
                    break;
                }
                case Perk.Dextrous:
                {
                    PlayerStats.ReloadSpeed.AddModifier(time => time * 0.667f);
                    PlayerStats.ThrowRate.AddModifier(rate => rate * 0.667f);
                    break;
                }
                case Perk.Resourceful:
                {
                    PlayerStats.ReloadSpeed.AddModifier(time => time * 0.5f);
                    break;
                }
                case Perk.Substantial:
                {
                    PlayerStats.PotionSize.AddModifier(size => size + 2);
                    break;
                }
                case Perk.Illuminating:
                {
                    PlayerStats.LightLevel.AddModifier(level => level + 1);
                    break;
                }
                case Perk.Magnetic:
                {
                    PlayerStats.PickupRadius.AddModifier(radius => radius * 2.5f);
                    break;
                }
                case Perk.Destructive:
                {
                    PlayerStats.ObjectDamage.AddModifier(damage => damage * 2f);
                    break;
                }
                case Perk.Gilded:
                {
                    PlayerStats.BonusCoins.AddModifier(coins => coins + 1);
                    break;
                }
                case Perk.Healthy:
                {
                    PlayerStats.MaxHealth.AddModifier(health => health + 2);
                    Player.Instance.Heal(2);
                    break;
                }
                case Perk.Evasive:
                {
                    PlayerStats.DodgeChance.AddModifier(chance => chance + 25f);
                    break;
                }
                case Perk.Swift:
                {
                    PlayerStats.MoveSpeed.AddModifier(speed => speed + 30f);
                    break;
                }
                case Perk.Invincible:
                {
                    PlayerStats.ImmunityPeriod.AddModifier(period => 3f);
                    break;
                }
            }
        }
    }

    public enum Perk
    {
        //Reaction
        Serrated,
        Afflictive,
        Effectual,
        Pungent,
        Incendiary,
        Superior,
        Dispersed,
        Dextrous,
        Contagious,

        //Experimentation
        Resourceful,
        Organised,
        Renewable,
        Efficient,
        Lingering,
        Substantial,
        Rhythmic,
        Saturated,
        Volatile,

        //Exploration
        Illuminating,
        Radar,
        Pathfinder,
        Magnetic,
        Cartographer,
        Destructive,
        Scavenger,
        Gilded,
        Exhilarated,

        //Survivability
        Healthy,
        Evasive,
        Swift,
        Athletic,
        Invincible,
        Thorny,
        Rejuvenating,
        Vampiric,
        Adrenalised
    }
}
