using System.Collections;
using Sigmoid.Reactions;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class SpiderAttacker : AttackerBase<SpiderParams>
    {
        private static WaitForSeconds _waitForSeconds0_4 = new WaitForSeconds(0.4f);

        public SpiderAttacker(Enemy enemy, SpiderParams parameters) : base(enemy, parameters)
        {
            attackSound = me.EnemyType.attackSound;
            targeting = (RandomTargeting) me.BaseTargeting;
            repather = (ManualRepather) targeting.repather;
            detonating = false;
            moveDelay = 0f;
        }

        private readonly ScriptableAudio attackSound;
        private readonly RandomTargeting targeting;
        private readonly ManualRepather repather;

        private int remainingMoves;
        private float moveDelay;
        private bool detonating;

        private IAttackable target;
        public override void Update(IAttackable target, float deltaTime)
        {
            this.target = target;
            float distanceToTarget = Vector2.Distance(me.transform.position, target.Position);
            if(!detonating && distanceToTarget <= 1.2f)
            {
                targeting.OverrideBias(1f);
                repather.Repath();
                detonating = true;
                moveDelay = 10f;

                if(attackSound) AudioManager.Instance.Play(attackSound, me.transform.position, AudioChannel.Enemy);
                me.StartCoroutine(Detonate());
            }

            if(!me.AtDestination || (moveDelay -= deltaTime) > 0f) return;
            if(remainingMoves <= 0)
            {
                remainingMoves = Random.Range(1, 5);
                moveDelay = Random.Range(0.8f, 2.4f);
                return;
            }

            targeting.OverrideBias(GetBias(distanceToTarget));
            repather.Repath();
            remainingMoves--;
            moveDelay = Random.Range(0.1f, 0.4f);
        }

        /// <summary>
        /// Returns max(0.25, 2^(dist/8)), a value for the directional bias
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public float GetBias(float distance) => Mathf.Max(0.25f, Mathf.Pow(2f, -0.125f * distance));

        private IEnumerator Detonate()
        {
            me.Animator.Play("Explode");
            yield return _waitForSeconds0_4;

            HitMask hitMask = target is PlayerAttackable ? HitMask.Players
                            : target is EnemyAttackable ? HitMask.Enemies
                            : HitMask.None;

            DetonatedPool explosionPool = ReactionPool.Instance.NuclearPool;
            explosionPool.Fetch().Initialise(explosionPool, me.transform.position, me, hitMask, 1f);
            me.Die();
        }
    }
}
