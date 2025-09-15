using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Tutorial
{
	public class LaserBeam : MonoBehaviour, IDamageSource
	{
        public string DisplayName => "Laser";

        [SerializeField] private Vector2 returnPosition;
        [SerializeField] private LineRenderer line;
        [SerializeField] private Material material;
        [SerializeField] private float oscillationFrequency;
        [SerializeField] private float oscillationAmplitude;

        private void Start() => line.material = new Material(material);
        private void Update()
        {
            float offset = oscillationAmplitude * Mathf.Tan(oscillationFrequency * Time.time);
            line.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(!other.TryGetComponent(out PlayerAttackable player)) return;
            
            DamageContext context = new DamageContext(0, DamageType.Light, DamageCategory.Blunt, this);
            player.ReceiveAttack(context);

            player.Position = returnPosition;
        }
	}
}
