using Sigmoid.Effects;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// A stationary dummy that can freely rotate, but will spring back to a normal rotation over time
    /// </summary>
	public class TargetDummy : MonoBehaviour
	{
        [field: SerializeField] public SpriteRenderer Sprite { get; private set; }
        [field: SerializeField] public Damageable Damageable { get; private set; }
        [field: SerializeField] public Rigidbody2D Body { get; private set; }
        [field: SerializeField] public MoveXY Mover { get; private set; }
		[SerializeField] private float springStrength;

		[SerializeField] private AudioPlayer player;
		[SerializeField] private Transform cloned;
		[SerializeField] private Transform copier;

        private void Awake()
        {
            Body.centerOfMass = Vector2.zero;
            Sprite.material = MaterialManager.ReplacementMaterial;

            Damageable.Initialise(int.MaxValue);
            Damageable.OnDamage += TakeDamage;
            if(Mover != null) Damageable.BuffReceiver.MoveSpeed.OnStatChanged += Mover.UpdateFrequency;
        }

        /// <summary>
        /// Plays a noise when taking damage
        /// </summary>
        /// <param name="damage"></param>
        private void TakeDamage(int damage) => player.Play();

        /// <summary>
        /// Applies a balancing torque to return the dummy to its original rotation
        /// </summary>
        private void Update()
		{
			Quaternion spring = Quaternion.FromToRotation(Body.transform.up, Vector2.up);
			float torque = spring.eulerAngles.z;
			if(torque > 180f) torque -= 360f;
			Body.angularVelocity = torque * springStrength;

			copier.localRotation = cloned.localRotation;
		}
	}
}
