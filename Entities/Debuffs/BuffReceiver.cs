using System.Collections.Generic;
using System.Linq;
using System;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Buffs
{
    /// <summary>
    /// Allows for enemies' stats to be modified through timed effects
    /// </summary>
    public class BuffReceiver : MonoBehaviour
    {
        public Stat<float> MoveSpeed;
        public Stat<float> DamageTaken;

        private void Awake() => Initialise();
        public void Initialise()
        {
            uniqueEffects = new Dictionary<object, TimedModifier>();
            MoveSpeed = new Stat<float>(1f);
            DamageTaken = new Stat<float>(1f);
        }

        /// <summary>
        /// Iterates over all unique effects, removing those that have expired
        /// </summary>
        private void Update()
        {
            foreach(object reaction in uniqueEffects.Keys.ToArray())
            {
                TimedModifier mod = uniqueEffects[reaction];
                mod.timer -= Time.deltaTime;
                if(mod.timer < 0f)
                {
                    mod.removal?.Invoke(mod.guid);
                    uniqueEffects.Remove(reaction);
                }
            }
        }

        private Dictionary<object, TimedModifier> uniqueEffects;
        public TimedModifier GetStoredModifier(object key) => uniqueEffects.TryGetValue(key, out TimedModifier mod) ? mod : null;
        public void Apply(object key, TimedModifier modifier) => uniqueEffects[key] = modifier;
    }

    /// <summary>
    /// Holds a removable, Guid-indexed effect that will remove after some amount of time
    /// </summary>
    public class TimedModifier
    {
        public readonly Guid guid;
        public readonly Action<Guid> removal;
        public float timer;

        public TimedModifier(Guid guid, Action<Guid> removalFunc)
        {
            this.guid = guid;
            removal = removalFunc;
        }
    }
}