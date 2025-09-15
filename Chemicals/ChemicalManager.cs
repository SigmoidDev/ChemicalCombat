using System.Collections.Generic;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Chemicals
{
	public class ChemicalManager : Singleton<ChemicalManager>
	{
		[SerializeField] private List<ScriptableChemical> chemicalList;
		private Dictionary<Chemical, ScriptableChemical> chemicalInfo;
		private Dictionary<Chemical, bool> discoveryStates;
        private static readonly Chemical[] indexedElements = {Chemical.Hydrogen, Chemical.Helium, Chemical.Carbon, Chemical.Oxygen, Chemical.Nitrogen, Chemical.Chlorine, Chemical.Sulphur, Chemical.Sodium, Chemical.Chromium, Chemical.Copper, Chemical.Iron, Chemical.Gold, Chemical.Neon, Chemical.Magnesium, Chemical.Silicon, Chemical.Uranium};

		private void Awake()
		{
			chemicalInfo = new Dictionary<Chemical, ScriptableChemical>();
			foreach(ScriptableChemical chemical in chemicalList)
				chemicalInfo.Add(chemical.relatedChemical, chemical);

			discoveryStates = new Dictionary<Chemical, bool>()
			{
				{ Chemical.Hydrogen,  true },
				{ Chemical.Helium,    true },
				{ Chemical.Carbon,    true },
				{ Chemical.Oxygen,    true },
				{ Chemical.Nitrogen,  true },
				{ Chemical.Chlorine,  true },
				{ Chemical.Sulphur,   false },
				{ Chemical.Sodium,    false },
				{ Chemical.Chromium,  false },
				{ Chemical.Copper,    false },
				{ Chemical.Iron,      false },
				{ Chemical.Gold,      false },
				{ Chemical.Neon,      false },
				{ Chemical.Magnesium, false },
				{ Chemical.Silicon,   false },
				{ Chemical.Uranium,   false }
			};
		}

        public static ScriptableChemical Get(Chemical chemical) => Instance.chemicalInfo.TryGetValue(chemical, out ScriptableChemical info) ? info : null;
        public static Chemical GetIndexed(int index) => indexedElements[index];
        public static Chemical GetRandom() => GetIndexed(Random.Range(0, 16));

        public event System.Action<Chemical> OnChemicalUnlocked;
		public static bool IsUnlocked(Chemical chemical) => Instance.discoveryStates.TryGetValue(chemical, out bool state) && state;
        public static void Unlock(Chemical chemical)
        {
            Instance.discoveryStates[chemical] = true;
            Instance.OnChemicalUnlocked?.Invoke(chemical);
        }
        public static void Lock(Chemical chemical)
        {
            Instance.discoveryStates[chemical] = false;
            Instance.OnChemicalUnlocked?.Invoke(chemical);
        }
    }

	/// <summary>
	/// Represents a single chemical type<br/>
	/// Uses prime numbers as keys (see CombinationVerifier)
	/// </summary>
	public enum Chemical
	{
		None = 0,
		Hydrogen = 2,
		Helium = 3,
		Carbon = 5,
		Oxygen = 7,
		Nitrogen = 11,
		Chlorine = 13,
		Sulphur = 17,
		Sodium = 19,
		Chromium = 23,
		Copper = 29,
		Iron = 31,
		Gold = 37,
		Neon = 41,
		Magnesium = 43,
		Silicon = 47,
		Uranium = 53
	}
}
