using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// Holds references to an ObjectPool for every type of reaction that needs one
    /// </summary>
	public class ReactionPool : Singleton<ReactionPool>
	{
        [field: Header("Explosions")]
		[field: SerializeField] public DetonatedPool FireworkPool { get; private set; }
		[field: SerializeField] public DetonatedPool FlashPool { get; private set; }
		[field: SerializeField] public DetonatedPool FusionPool { get; private set; }
		[field: SerializeField] public DetonatedPool ThermitePool { get; private set; }
		[field: SerializeField] public DetonatedPool GunpowderPool { get; private set; }
		[field: SerializeField] public DetonatedPool NuclearPool { get; private set; }
		[field: SerializeField] public DetonatedPool CloudPool { get; private set; }
        [field: Space]

        [field: Header("Spawns")]
		[field: SerializeField] public SpawnedPool AcidPool { get; private set; }
		[field: SerializeField] public SpawnedPool CatalysePool { get; private set; }
		[field: SerializeField] public SpawnedPool CrystalPool { get; private set; }
		[field: SerializeField] public SpawnedPool MagmaPool { get; private set; }
		[field: SerializeField] public SpawnedPool PlasmaPool { get; private set; }
		[field: SerializeField] public SpawnedPool StinkPool { get; private set; }
		[field: SerializeField] public SpawnedPool SludgePool { get; private set; }
        [field: Space]

        [field: Header("Effects")]
		[field: SerializeField] public VisualPool SplashPool { get; private set; }
		[field: SerializeField] public VisualPool RagePool { get; private set; }
		[field: SerializeField] public VisualPool ConfusedPool { get; private set; }
		[field: SerializeField] public TrackingPool BalloonPool { get; private set; }
		[field: SerializeField] public TrackingPool IcePool { get; private set; }
        [field: Space]

        [field: Header("Projectiles")]
		[field: SerializeField] public Projectile DiamondProjectile { get; private set; }
		[field: SerializeField] public Projectile GlassProjectile { get; private set; }
		[field: SerializeField] public Projectile LightningBolt { get; private set; }
        [field: Space]

        [field: Header("Other")]
        [field: SerializeField] public RandomParams DrunkParams { get; private set; }
	}
}
