using System.Collections.Generic;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Create New Enemy")]
	public class ScriptableEnemy : ScriptableObject
	{
        [Header("Info")]
		[Min(1)] public int health;
		[Min(0)] public int minCoins;
		[Min(0)] public int maxCoins;
        public int NumCoins => Random.Range(minCoins, maxCoins + 1);
		public RuntimeAnimatorController animator;
        public Sprite shadow;
        [Space]

        [Header("Behaviour")]
        public ScriptableHitbox hitbox;
        public AccelerationType acceleration;
        public AccelerationType turning;
        public bool alwaysUpdate;
        public float mass = 1f;
        [Space]

        public MovementParams movement;
        public TargetingParams targeting;
        public AttackerParams attacker;

        [Header("Sounds")]
        public ScriptableAudio randomSound;
        public Vector2 soundInterval;
        public ScriptableAudio attackSound;
        public ScriptableAudio hurtSound;
        public ScriptableAudio deathSound;
        public ScriptableAudio otherSound;
        [Space]

        [Header("Effectivenesses")]
		public List<Effectiveness<DamageType>> damageEffectivenesses;
		public List<Effectiveness<DamageCategory>> categoryEffectivenesses;

        public float GetTypeEffectiveness(DamageType type)
        {
            foreach(Effectiveness<DamageType> effectiveness in damageEffectivenesses)
                if(effectiveness.thing == type) return effectiveness.effectiveness;

            return 1f;
        }

        public float GetCategoryEffectiveness(DamageCategory category)
        {
            foreach(Effectiveness<DamageCategory> effectiveness in categoryEffectivenesses)
                if(effectiveness.thing == category) return effectiveness.effectiveness;

            return 1f;
        }
	}

    /// <summary>
    /// A factor determining how effective a given enum type is against something<br/>
    /// Will be applied multiplicatively when calculating final damage
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class Effectiveness<T> where T : System.Enum
	{
		public T thing;
		[Range(0f, 2f)] public float effectiveness = 1f;

        public override string ToString() => thing.ToString() + " x" + effectiveness;
	}
}
