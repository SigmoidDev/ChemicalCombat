namespace Sigmoid.Generation
{
    /// <summary>
    /// Class to add random noise and to smooth rooms
    /// </summary>
	public static class CellularAutomata
	{
		/// <summary>
		/// Randomly flips some tiles based on the noise percentage
		/// </summary>
		/// <param name="dungeon"></param>
		/// <param name="noise"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static Dungeon Noisify(Dungeon dungeon, int noise, int seed)
		{
			System.Random noisifier = new System.Random(seed);
			for(int x = dungeon.Padding.left; x < dungeon.Size.x + dungeon.Padding.left; x++)
			{
				for(int y = dungeon.Padding.bottom; y < dungeon.Size.y + dungeon.Padding.bottom; y++)
				{
					if(noisifier.Next(0, 100) < noise)
					{
						dungeon.Map[x, y] = !dungeon.Map[x, y];
					}
				}
			}
			return dungeon;
		}



		/// <summary>
		/// Runs a single pass of cellular automata on the grid
		/// </summary>
		/// <param name="dungeon"></param>
		/// <param name="smoothness"></param>
		/// <param name="underpopulationNumber"></param>
		/// <param name="overpopulationNumber"></param>
		/// <returns></returns>
		public static Dungeon Smoothen(Dungeon dungeon, int smoothness, int underpopulationNumber, int overpopulationNumber)
		{
			for(int pass = 0; pass < smoothness; pass++)
			{
				for(int x = dungeon.Padding.left; x < dungeon.Size.x + dungeon.Padding.left; x++)
				{
					for(int y = dungeon.Padding.bottom; y < dungeon.Size.y + dungeon.Padding.bottom; y++)
					{
						int numNeighbours = GetNeighbourCount(dungeon.Map, x, y);
						if(!dungeon.Map[x, y] && numNeighbours < underpopulationNumber) dungeon.Map[x, y] = true;
						if(dungeon.Map[x, y] && numNeighbours > overpopulationNumber) dungeon.Map[x, y] = false;
					}
				}
			}
			return dungeon;
		}



		/// <summary>
		/// Counts the number of neighbours that a given cell has
		/// </summary>
		/// <param name="map"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>An integer from 0 to 9</returns>
		private static int GetNeighbourCount(bool[,] map, int x, int y)
		{
			int neighbours = 0;
			for(int xx = -1; xx <= 1; xx++)
			{
				for(int yy = -1; yy <= 1; yy++)
				{
					if(!map[x + xx, y + yy])
					{
						neighbours++;
					}
				}
			}
			return neighbours;
		}
	}
}
