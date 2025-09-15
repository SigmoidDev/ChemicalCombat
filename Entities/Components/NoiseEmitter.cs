using UnityEngine;

namespace Sigmoid.Enemies
{
	public class NoiseEmitter : MonoBehaviour
	{
		[SerializeField] private SoundEffect attackSound;
		[SerializeField] private SoundEffect hitSound;
		[SerializeField] private SoundEffect deathSound;
		[SerializeField] private SoundEffect timedSound;

        [SerializeField] private float intervalMin;
        [SerializeField] private float intervalMax;
        private float cooldown;

        private void Awake()
        {
            //subscribe to events
        }

        private void Update()
        {
            if(timedSound == null) return;

            cooldown -= Time.deltaTime;
            if(cooldown <= 0f)
            {
                timedSound.Play();
                cooldown = Random.Range(intervalMin, intervalMax);
            }
        }

        public void Attack(){ attackSound?.Play(); }
        public void Hit(){    hitSound?.Play();    }
        public void Die(){    deathSound?.Play();  }
	}

    [System.Serializable]
    public class SoundEffect
    {
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume;

        public void Play()
        {
            
        }
    }
}
