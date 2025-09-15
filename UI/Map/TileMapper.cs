namespace Sigmoid.UI
{
	public static class TileMapper
	{
        /// <summary>
        /// Gets the desired palette index based on the map provided, and an optional inversion
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="map"></param>
        /// <param name="inverted"></param>
        /// <param name="highlights"></param>
        /// <returns></returns>
		public static int GetColourIndex(int x, int y, int width, int height, bool[,] map, bool inverted, bool[,] highlights)
		{
            //Using != here functions like an XOR
			bool inRoom = map[x, y] != inverted;
			if(inRoom) return 1;

			bool aboveRoom = y != 0 && map[x, y - 1] != inverted;
			if(aboveRoom) return 2;

			for(int xx = -1; xx <= 1; xx++)
			{
				for(int yy = -2; yy <= 1; yy++)
				{
					int newX = x + xx;
					if(newX < 0 || newX >= width) continue;

					int newY = y + yy;
					if(newY < 0 || newY >= height) continue;

					if(map[newX, newY] != inverted) return highlights[x, y] ? 0 : 3;
				}
			}
			return -1;
		}
	}
}