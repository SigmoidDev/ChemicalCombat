#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sigmoid.Players
{
    [CustomEditor(typeof(PlayerStats))]
	public class PlayerInspector : Editor
	{
        public override void OnInspectorGUI()
        {
            RenderStat("Health", PlayerStats.MaxHealth, " HP");
            RenderStat("Dodge", PlayerStats.DodgeChance, "%");
            RenderStat("Speed", PlayerStats.MoveSpeed, "%");
            RenderStat("Immunity", PlayerStats.ImmunityPeriod, "s");
            RenderStat("Damage", PlayerStats.BaseDamage);
            RenderStat("Throw", PlayerStats.ThrowRate, "x");
            RenderStat("Reload", PlayerStats.ReloadSpeed, "x");
            RenderStat("Splash", PlayerStats.SplashRadius, "m");
            RenderStat("Crit", PlayerStats.CritChance, "%");
            RenderStat("Explosions", PlayerStats.ExplosionRadius, "m");
            RenderStat("DoTs", PlayerStats.DoTRate, "x");
            RenderStat("Debuffs", PlayerStats.DebuffDuration, "x");
            RenderStat("Effects", PlayerStats.EffectDuration, "x");
            RenderStat("Size", PlayerStats.PotionSize);
            RenderStat("Light", PlayerStats.LightLevel);
            RenderStat("Pickup", PlayerStats.PickupRadius, "m");
            RenderStat("Objects", PlayerStats.ObjectDamage, "x");
            RenderStat("Coins", PlayerStats.BonusCoins);

        }

        public void RenderStat<T>(string label, Stat<T> stat, string suffix = "") where T : struct
        {
            string value = "Null";
            if(stat != null) value = stat.Value.ToString() + suffix;
            GUILayout.Label(label + ": " + value);
        }
	}
}
#endif