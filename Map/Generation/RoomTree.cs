using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;

namespace Sigmoid.Generation
{
	/// <summary>
	/// The half of room responsible for generating the Binary Tree
	/// </summary>
	public partial class RoomNode
	{
		/// <summary>
		/// The minimum size of each subroom as a percentage of its parent<br/>
		/// Values closer to 50% will result in more uniform dungeons<br/>
		/// Must be less than 50% as both halves cannot add up to more than 100%
		/// </summary>
		public const float MIN_PERCENT = 42;

		/// <summary>
		/// The minimum size of each subdivision in tiles
		/// </summary>
		public const float MIN_ABSOLUTE = 14;



		public int level;
		public RectInt bounds;
		public RectInt interior;
		public RoomNode roomA;
		public RoomNode roomB;



		/// <summary>
		/// Recursively generates a tree of rooms based on the depth parameter provided
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="depth"></param>
		/// <param name="random"></param>
		public RoomNode(RectInt bounds, int depth, int max, Random random)
		{
			this.bounds = bounds;
			level = max - depth;

			if(depth <= 0)
			{
				//If the room is more than 40% bigger in one direction than the other
				bool tooTall = bounds.height > bounds.width * 1.4f;
				bool tooWide = bounds.width > bounds.height * 1.4f;

				bool bigEnough = false;
				while(!bigEnough)
				{
					//Attempts to make the rooms more square if they were weirdly long
					int innerWidth = (int) (bounds.width * random.Next(tooTall ? 100 : tooWide ? 50 : 75, tooWide ? 80 : 100) * 0.01f);
					int innerHeight = (int) (bounds.height * random.Next(tooWide ? 100 : tooTall ? 50 : 75, tooTall ? 80 : 100) * 0.01f);

					if(innerWidth >= MIN_ABSOLUTE && innerHeight >= MIN_ABSOLUTE) bigEnough = true;

					int xOffset = random.Next(0, bounds.width - innerWidth);
					int yOffset = random.Next(0, bounds.height - innerHeight);

					interior = new RectInt(bounds.x + xOffset, bounds.y + yOffset, innerWidth, innerHeight);
					return;
				}
			}

			//We'll want to keep attempting to split the rooms so as not to create blank spaces
			int attempts = 0;
			bool successful = false;
			while(!successful && attempts < 4)
			{
				//Chooses whichever side is longer for more uniform shapes
				if(bounds.width > bounds.height) successful = SplitHorizontally(bounds, depth, max, random);
				else successful = SplitVertically(bounds, depth, max, random);
				attempts++;
			}
		}

		/// <summary>
		/// Splits the room a random amount horizontally
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="depth"></param>
		/// <param name="random"></param>
		/// <returns>Whether it was a success</returns>
		private bool SplitHorizontally(RectInt bounds, int depth, int max, Random random)
		{
			int minX = (int) (bounds.x + bounds.width * MIN_PERCENT * 0.01f);
			int maxX = (int) (bounds.x + bounds.width * (1 - MIN_PERCENT * 0.01f));
			if(minX >= maxX) return false;

			int division = random.Next(minX, maxX);
			int leftWidth = division - bounds.x;
			int rightWidth = bounds.x + bounds.width - division;
			if(leftWidth < MIN_ABSOLUTE || rightWidth < MIN_ABSOLUTE) return false;

			RectInt leftRect = new RectInt(bounds.x, bounds.y, leftWidth, bounds.height);
			RectInt rightRect = new RectInt(division, bounds.y, rightWidth, bounds.height);

			roomA = new RoomNode(leftRect, depth - 1, max, random);
			roomB = new RoomNode(rightRect, depth - 1, max, random);
			return true;
		}

		/// <summary>
		/// Splits the room a random amount verically
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="depth"></param>
		/// <param name="random"></param>
		/// <returns>Whether it was a success</returns>
		private bool SplitVertically(RectInt bounds, int depth, int max, Random random)
		{
			int minY = (int) (bounds.y + bounds.height * MIN_PERCENT * 0.01f);
			int maxY = (int) (bounds.y + bounds.height * (1 - MIN_PERCENT * 0.01f));
			if(minY >= maxY) return false;

			int division = random.Next(minY, maxY);
			int lowerHeight = division - bounds.y;
			int upperHeight = bounds.y + bounds.height - division;
			if(lowerHeight < MIN_ABSOLUTE || upperHeight < MIN_ABSOLUTE) return false;

			RectInt lowerRect = new RectInt(bounds.x, bounds.y, bounds.width, lowerHeight);
			RectInt upperRect = new RectInt(bounds.x, division, bounds.width, upperHeight);

			roomA = new RoomNode(lowerRect, depth - 1, max, random);
			roomB = new RoomNode(upperRect, depth - 1, max, random);
			return true;
		}



		/// <summary>
		/// Determines whether or not a room has subrooms<br/>
		/// i.e. is not of the smallest size available
		/// </summary>
		/// <returns></returns>
		public bool HasRooms() => roomA != null && roomB != null;

		/// <summary>
		/// Gets a list of ALL children in the tree
		/// </summary>
		/// <returns></returns>
		public List<RoomNode> GetRooms()
		{
			List<RoomNode> rooms = new List<RoomNode>();
			if(!HasRooms()) rooms.Add(this);
			else
			{
				rooms.AddRange(roomA.GetRooms());
				rooms.AddRange(roomB.GetRooms());
			}

			return rooms;
		}
	}
}
