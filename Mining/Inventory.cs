using System.Collections.Generic;
using Sigmoid.Utilities;
using Sigmoid.Chemicals;

namespace Sigmoid.Mining
{
	public class Inventory : Singleton<Inventory>
	{
		private Dictionary<Chemical, int> stock;
		private void Awake() => stock = new Dictionary<Chemical, int>
		{
			{ Chemical.Hydrogen,  32 },
			{ Chemical.Helium,    32 },
			{ Chemical.Carbon,    16 },
			{ Chemical.Oxygen,    16 },
			{ Chemical.Nitrogen,  8 },
			{ Chemical.Chlorine,  8 },
			{ Chemical.Sulphur,   0 },
			{ Chemical.Sodium,    0 },
			{ Chemical.Chromium,  0 },
			{ Chemical.Copper,    0 },
			{ Chemical.Iron,      0 },
			{ Chemical.Gold,      0 },
			{ Chemical.Neon,      0 },
			{ Chemical.Magnesium, 0 },
			{ Chemical.Silicon,   0 },
			{ Chemical.Uranium,   0 }
		};

        public int AmountOf(Chemical chemical) => stock.TryGetValue(chemical, out int amount) ? amount : 0;
		public bool HasAny(Chemical chemical) => AmountOf(chemical) > 0;

		public void AddStock(Chemical chemical, int amount) => stock[chemical] += amount;
		public void UseStock(Chemical chemical, int amount) => stock[chemical] -= amount;
    }
}
