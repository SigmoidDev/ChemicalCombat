using System.Collections;
using Sigmoid.Enemies;
using Sigmoid.Buffs;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// An explosion effect which inflicts a stun debuff on top of dealing damage
    /// </summary>
	public class Flash : Explosion
	{
		[SerializeField] private UnityEngine.Rendering.Universal.Light2D emitter;

        protected override float EffectiveRadius => 1.25f;
        protected override void Effect(IAttackable attackable)
        {
            base.Effect(attackable);
            if(attackable is DamageableAttackable damageable)
            {
                TimedModifier stun = damageable.Damageable.BuffReceiver.GetStoredModifier(this);
                stun ??= new TimedModifier(
                    damageable.Damageable.BuffReceiver.MoveSpeed.AddModifier(speed => speed * 0f),
                    guid => damageable.Damageable.BuffReceiver.MoveSpeed.RemoveModifier(guid)
                );

                stun.timer = 1.5f;
                damageable.Damageable.BuffReceiver.Apply(this, stun);
            }
        }

        protected override IEnumerator OnDetonated()
        {
            foreach(ScriptableAudio sound in sounds)
                AudioManager.Instance.Play(sound, transform.position, AudioChannel.Sound);

            for(float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
			{
				emitter.intensity = Mathf.Lerp(emitter.intensity, 0f, Time.deltaTime * 10f);
				yield return null;
			}

			pool.Release(this);
        }
    }
}
