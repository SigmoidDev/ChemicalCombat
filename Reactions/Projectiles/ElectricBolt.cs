using System.Collections.Generic;
using System.Collections;
using Sigmoid.Projectiles;
using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class ElectricBolt : Projectile
	{
        private static WaitForSeconds _waitForSeconds0_15 = new WaitForSeconds(0.15f);
        private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);

        public override IDamageSource Source => Player.Instance;

		[SerializeField] private LineRenderer line;
        [SerializeField] private Material material;
        [SerializeField] private Texture2D[] textures;
        [SerializeField] private ScriptableAudio sound;
        private ProjectilePool pool;

        private float elapsed;
        private int index;
        private void Update()
        {
            if((elapsed += Time.deltaTime) > 0.05f)
            {
                elapsed -= 0.05f;
                index = (index + 1) % 4;
                line.material.SetTexture("_MainTex", textures[index]);
            }
        }

        public void Initialise(ProjectilePool pool, DamageableAttackable target)
        {
            this.pool = pool;
            StartCoroutine(RicochetOverTime(target));
        }

        private IEnumerator RicochetOverTime(DamageableAttackable target)
        {
            line.positionCount = 1;
            line.SetPosition(0, target.transform.position + 0.5f * Vector3.up);

            current = target;
            path = new HashSet<DamageableAttackable>{ current };
            Zap(current);

            AudioManager.Instance.Play(sound, current.Position, AudioChannel.Sound);
            Debug.Log("Hello?");

            for(int i = 0; i < 4; i++)
            {
                DamageableAttackable next = Ricochet();
                if(next == null) break;

                path.Add(next);
                current = next;
                Zap(current);

                line.positionCount = i + 2;
                line.SetPosition(i + 1, current.transform.position + 0.5f * Vector3.up);
                yield return _waitForSeconds0_1;
            }

            yield return _waitForSeconds0_15;
            pool.Release(this);
        }

        [SerializeField] private LayerMask hitboxMask;
        private ContactFilter2D filter;
        private Collider2D[] buffer;
        private DamageableAttackable current;
        private HashSet<DamageableAttackable> path;

        private void Awake()
        {
            buffer = new Collider2D[10];
            filter = new ContactFilter2D();
            filter.SetLayerMask(hitboxMask);
            line.material = new Material(material);
        }

        public DamageableAttackable Ricochet()
        {
            DamageableAttackable target = null;
            float minDistance = Mathf.Infinity;

            int numHits = Physics2D.OverlapCircle(current.transform.position, 2f, filter, buffer);
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out DamageableAttackable other)
                || other == current || path.Contains(other)) continue;

                float distance = Vector2.Distance(current.transform.position, other.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    target = other;
                }
            }

            return target;
        }

        public void Zap(DamageableAttackable damageable) => damageable.ReceiveAttack(new DamageContext(6, DamageType.Electric, DamageCategory.Blunt, Source));

        public override void Collide(Collider2D other){}
        public override void Hit(IAttackable attackable){}
    }
}
