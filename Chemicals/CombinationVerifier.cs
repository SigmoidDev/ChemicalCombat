using System.Collections.Generic;
using Sigmoid.Reactions;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Chemicals
{
	public class CombinationVerifier : Singleton<CombinationVerifier>
	{
        private List<Reaction> reactionList;
		private Dictionary<Combination, Reaction> reactions;
        private void Awake()
        {
            //Ensures that there is only one instance of each reaction
            CaffeineReaction caffeine = new CaffeineReaction();
            MidasReaction midas = new MidasReaction();
            SteelReaction steel = new SteelReaction();

            AlcoholReaction alcohol = new AlcoholReaction();
            FreezeReaction freeze = new FreezeReaction();
            GlowingReaction glowing = new GlowingReaction();
            LevitateReaction levitate = new LevitateReaction();
            SaltReaction salt = new SaltReaction();
            WaterReaction water = new WaterReaction();

            BleachReaction bleach = new BleachReaction();
            CorrosionReaction corrosion = new CorrosionReaction();
            CyanideReaction cyanide = new CyanideReaction();
            PoisonReaction poison = new PoisonReaction();

            FireworksReaction fireworks = new FireworksReaction();
            FlashReaction flash = new FlashReaction();
            FusionReaction fusion = new FusionReaction();
            GunpowderReaction gunpowder = new GunpowderReaction();
            NuclearReaction nuclear = new NuclearReaction();
            ThermiteReaction thermite = new ThermiteReaction();

            DiamondReaction diamond = new DiamondReaction();
            ElectricityReaction electricity = new ElectricityReaction();
            GlassReaction glass = new GlassReaction();
            //PyriteReaction pyrite = new PyriteReaction();
            RubberReaction rubber = new RubberReaction();
            WindReaction wind = new WindReaction();

            //MarbleReaction marble = new MarbleReaction();
            AcidReaction acid = new AcidReaction();
            CatalyseReaction catalyse = new CatalyseReaction();
            CrystalReaction crystal = new CrystalReaction();
            MagmaReaction magma = new MagmaReaction();
            PlasmaReaction plasma = new PlasmaReaction();
            StinkReaction stink = new StinkReaction();

            //GolemReaction golem = new GolemReaction();
            //PlantReaction plant = new PlantReaction();
            //PrismReaction prism = new PrismReaction();
            //RobotReaction robot = new RobotReaction();

            reactionList = new List<Reaction>{ caffeine, midas, steel, alcohol, freeze, glowing, levitate, salt, water, bleach, corrosion, cyanide, poison, fireworks, flash, fusion, gunpowder, nuclear, thermite, diamond, electricity, glass, rubber, wind, acid, catalyse, crystal, magma, plasma, stink };
            reactions = new Dictionary<Combination, Reaction>(new CombinationComparer())
			{
                //Buffs
                { new Combination(Chemical.Carbon,     Chemical.Hydrogen,    Chemical.Nitrogen),     caffeine },
                { new Combination(Chemical.Gold,       Chemical.Gold,        Chemical.Gold),         midas },
                { new Combination(Chemical.Copper,     Chemical.Iron,        Chemical.Gold),         midas },
                { new Combination(Chemical.Iron,       Chemical.Carbon),     steel },
                { new Combination(Chemical.Iron,       Chemical.Iron),       steel },

                //Debuffs
                { new Combination(Chemical.Carbon,     Chemical.Hydrogen,    Chemical.Chlorine),     alcohol },
                { new Combination(Chemical.Nitrogen,   Chemical.Nitrogen),   freeze },
                { new Combination(Chemical.Nitrogen,   Chemical.Helium),     freeze },
                { new Combination(Chemical.Neon,       Chemical.Neon),       glowing },
				{ new Combination(Chemical.Neon,       Chemical.Uranium),    glowing },
				{ new Combination(Chemical.Oxygen,     Chemical.Uranium),    glowing },
                { new Combination(Chemical.Hydrogen,   Chemical.Helium),     levitate },
				{ new Combination(Chemical.Oxygen,     Chemical.Helium),     levitate },
                { new Combination(Chemical.Sodium,     Chemical.Chlorine),   salt },
				{ new Combination(Chemical.Magnesium,  Chemical.Sulphur),    salt },
                { new Combination(Chemical.Hydrogen,   Chemical.Oxygen),     water },

                //DoTs
                { new Combination(Chemical.Sodium,     Chemical.Chlorine,    Chemical.Oxygen),       bleach },
                { new Combination(Chemical.Chlorine,   Chemical.Copper),     corrosion },
				{ new Combination(Chemical.Chlorine,   Chemical.Iron),       corrosion },
                { new Combination(Chemical.Carbon,     Chemical.Nitrogen),   cyanide },
                { new Combination(Chemical.Chlorine,   Chemical.Chlorine),   poison },
				{ new Combination(Chemical.Sulphur,    Chemical.Sulphur),    poison },
				{ new Combination(Chemical.Carbon,     Chemical.Oxygen),     poison },

                //Explosions
                { new Combination(Chemical.Sulphur,    Chemical.Carbon,      Chemical.Copper),       fireworks },
                { new Combination(Chemical.Sulphur,    Chemical.Carbon,      Chemical.Chromium),     fireworks },
				{ new Combination(Chemical.Magnesium,  Chemical.Magnesium),  flash },
                { new Combination(Chemical.Iron,       Chemical.Magnesium),  flash },
				{ new Combination(Chemical.Hydrogen,   Chemical.Hydrogen),   fusion },
				{ new Combination(Chemical.Helium,     Chemical.Helium),     fusion },
				{ new Combination(Chemical.Sulphur,    Chemical.Carbon),     gunpowder },
				{ new Combination(Chemical.Sulphur,    Chemical.Nitrogen),   gunpowder },
				{ new Combination(Chemical.Uranium,    Chemical.Uranium),    nuclear },
				{ new Combination(Chemical.Uranium,    Chemical.Hydrogen),   nuclear },
				{ new Combination(Chemical.Iron,       Chemical.Oxygen),     thermite },
				{ new Combination(Chemical.Magnesium,  Chemical.Oxygen),     thermite },

                //Projectiles
				{ new Combination(Chemical.Carbon,     Chemical.Carbon),     diamond },
				{ new Combination(Chemical.Copper,     Chemical.Copper),     electricity },
				{ new Combination(Chemical.Copper,     Chemical.Gold),       electricity },
				{ new Combination(Chemical.Silicon,    Chemical.Silicon),    glass },
				{ new Combination(Chemical.Silicon,    Chemical.Oxygen),     glass },
				//{ new Combination(Chemical.Iron,       Chemical.Sulphur),    pyrite },
				//{ new Combination(Chemical.Gold,       Chemical.Sulphur),    pyrite },
                { new Combination(Chemical.Hydrogen,   Chemical.Carbon),     rubber },
				{ new Combination(Chemical.Oxygen,     Chemical.Oxygen),     wind },
                { new Combination(Chemical.Nitrogen,   Chemical.Oxygen),     wind },

                //Spawns
				//{ new Combination(Chemical.Carbon,     Chemical.Magnesium,   Chemical.Silicon),      marble },
				{ new Combination(Chemical.Hydrogen,   Chemical.Chlorine),   acid },
				{ new Combination(Chemical.Hydrogen,   Chemical.Sulphur),    acid },
                { new Combination(Chemical.Sodium,     Chemical.Nitrogen),   catalyse },
                { new Combination(Chemical.Sodium,     Chemical.Carbon),     catalyse },
                { new Combination(Chemical.Silicon,    Chemical.Chromium),   crystal },
                { new Combination(Chemical.Silicon,    Chemical.Carbon),     crystal },
				{ new Combination(Chemical.Helium,     Chemical.Carbon),     magma },
				{ new Combination(Chemical.Helium,     Chemical.Silicon),    magma },
                { new Combination(Chemical.Helium,     Chemical.Neon),       plasma },
				{ new Combination(Chemical.Sulphur,    Chemical.Oxygen),     stink },
				{ new Combination(Chemical.Sulphur,    Chemical.Chlorine),   stink }

                //Summons
				//{ new Combination(Chemical.Iron,       Chemical.Silicon,     Chemical.Carbon),       golem },
				//{ new Combination(Chemical.Carbon,     Chemical.Nitrogen,    Chemical.Magnesium),    plant },
				//{ new Combination(Chemical.Chromium,   Chemical.Hydrogen,    Chemical.Silicon),      prism },
				//{ new Combination(Chemical.Chromium,   Chemical.Hydrogen,    Chemical.Oxygen),       prism },
				//{ new Combination(Chemical.Silicon,    Chemical.Copper,      Chemical.Copper),       robot },
				//{ new Combination(Chemical.Silicon,    Chemical.Copper,      Chemical.Gold),         robot }
			};
        }

		private void Update()
		{
			foreach(Reaction reaction in reactionList)
				reaction.Update(Time.deltaTime);
		}

		/// <summary>
		/// Attempts to get the Reaction associated with this combination of chemicals, or else returns null if it was not found
		/// </summary>
		/// <param name="combination"></param>
		/// <returns></returns>
        public static Reaction Try(Combination combination) => Instance.reactions.TryGetValue(combination, out Reaction reaction) ? reaction : null;
    }

	/// <summary>
	/// Holds a unique key generated by multiplying together a list of prime numbers<br/>
	/// Since Chemical.None has a value of 0, this will make any inert reactions immediately 0
	/// </summary>
	public readonly struct Combination
	{
		public readonly int uniqueKey;

		public Combination(Chemical chemicalA, Chemical chemicalB) : this(new List<Chemical>{ chemicalA, chemicalB }){}
		public Combination(Chemical chemicalA, Chemical chemicalB, Chemical chemicalC) : this(new List<Chemical>{ chemicalA, chemicalB, chemicalC }){}
		public Combination(List<Chemical> chemicals)
		{
			uniqueKey = 1;
			foreach(Chemical chemical in chemicals)
				uniqueKey *= (int) chemical;
		}
    }

    public class CombinationComparer : IEqualityComparer<Combination>
    {
        public bool Equals(Combination a, Combination b) => a.uniqueKey == b.uniqueKey;
        public int GetHashCode(Combination obj) => base.GetHashCode();
    }
}
