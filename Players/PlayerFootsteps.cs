using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Audio;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Players
{
	public class PlayerFootsteps : MonoBehaviour
	{
		[SerializeField] private AudioPlayer footstep1;
		[SerializeField] private AudioPlayer footstep2;
        [SerializeField] private float footstepInterval;
        private bool alternate;
        private AudioPlayer CurrentFootstep => alternate ? footstep2 : footstep1;
        private float audioTimer = 0f;

		[SerializeField] private ParticleSystem particles;
        private float particleTimer = 0f;

        private void Update()
        {
            audioTimer -= Time.deltaTime;
            particleTimer -= Time.deltaTime;

            float movement = Options.Keybinds.GetMovement().Magnitude1D();
            if(movement <= 0.01f || Player.Instance.Movement.IsSliding) return;

            if(audioTimer <= 0f)
            {
                bool isSprinting = Input.GetKey(Options.Keybinds[Key.Sprint]);
                audioTimer = footstepInterval * (isSprinting ? 0.8f : 1f);

                CurrentFootstep.Play();
                alternate = !alternate;
            }

            if(particleTimer <= 0f)
            {
                particleTimer = 0.05f;
                particles.Play();
            }
        }
	}
}
