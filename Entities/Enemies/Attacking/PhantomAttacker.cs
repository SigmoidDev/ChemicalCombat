using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Complex controller for the Phantom, allowing it to inflict the player with the Terrified debuff
    /// </summary>
	public class PhantomAttacker : AttackerBase<PhantomParams>
    {
        public PhantomAttacker(Enemy enemy, PhantomParams parameters) : base(enemy, parameters) => spookSound = enemy.EnemyType.attackSound;

        private readonly ScriptableAudio spookSound;
        private float spookTimer = 0f;

        private bool hasIgnored = false;
        private int previousStacks = 0;
        public override void Update(IAttackable target, float deltaTime)
        {
            if(target is not PlayerAttackable)
            {
                if(!hasIgnored)
                {
                    hasIgnored = true;
                    PlayerBuffs.Instance.UpdateTerrifiedStacks(this, 0);
                }
                return;
            }

            hasIgnored = false;
            spookTimer -= deltaTime;

            Vector2 myPosition = me.transform.position;
            Vector2 targetPosition = target.Position;
            float distance = Vector2.Distance(myPosition, targetPosition);

            int stacks;
            if(distance > 4.0f) stacks = 0;
            else if(distance > 2.5f) stacks = 1;
            else if(distance > 1.0f) stacks = 2;
            else stacks = 3;

            if(stacks != previousStacks)
            {
                PlayerBuffs.Instance.UpdateTerrifiedStacks(this, stacks);
                if(stacks == 3 && spookTimer <= 0f)
                {
                    spookTimer = 2.0f;
                    if(spookSound) AudioManager.Instance.Play(spookSound, me.transform.position, AudioChannel.Enemy);
                }
            }

            previousStacks = stacks;
        }

        public override void Destroy() => PlayerBuffs.Instance.RemoveTerrifiedStacks(this);
    }
}
