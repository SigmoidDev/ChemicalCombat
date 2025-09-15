using System.Collections;
using UnityEngine.AI;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class WizardAttacker : ProjectileAttacker
    {
        private static WaitForSeconds _waitForSeconds0_35 = new WaitForSeconds(0.35f);

        public WizardAttacker(Enemy enemy, WizardParams parameters) : base(enemy, parameters)
        {
            teleportCooldown = 0f;
            randomness = 0.5f;

            enemy.Damageable.OnDamage += TeleportAway;
            enemy.OnDeath += Unsubscribe;
        }

        private void Unsubscribe(Enemy enemy)
        {
            enemy.Damageable.OnDamage -= TeleportAway;
            enemy.OnDeath -= Unsubscribe;
        }

        private float teleportCooldown;
        public override void Update(IAttackable target, float deltaTime)
        {
            base.Update(target, deltaTime);
            teleportCooldown -= deltaTime;
        }

        private void TeleportAway(int damage)
        {
            if(teleportCooldown <= 0f)
            {
                teleportCooldown = 1.5f;
                me.StartCoroutine(Teleport());
            }
        }

        private readonly float randomness;
        private IEnumerator Teleport()
        {
            timer = cooldown;
            me.Animator.Play("Teleport");
            yield return _waitForSeconds0_35;

            Vector2 myPosition = me.transform.position;
            Vector2 deltaPosition = target.Position - myPosition;
            Vector2 randomPosition = new Vector2(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
            Vector2 retreatPosition = target.Position + 3f * deltaPosition.normalized + randomPosition;

            if(!NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                randomPosition = new Vector2(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
                retreatPosition = target.Position + 1.5f * deltaPosition.normalized + randomPosition;
                hit.position = retreatPosition;
            }

            me.transform.position = hit.position;
        }
    }
}
