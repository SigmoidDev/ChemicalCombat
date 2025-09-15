using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Sigmoid.Generation
{
	[CreateAssetMenu(fileName = "New Rule Tile", menuName = "Dungeons/Create New Rule Tile")]
	public class CustomTile : RuleTile<CustomTile.Neighbor>
	{
    	public List<TileBase> siblings = new List<TileBase>();
    	public class Neighbor : TilingRule.Neighbor
		{
    	    public const int Sibling = 3;
    	}

    	public override bool RuleMatch(int neighbor, TileBase tile)
		{
			switch(neighbor)
			{
				case Neighbor.Sibling:
					return tile == this || siblings.Contains(tile);
			}

			return base.RuleMatch(neighbor, tile);
    	}
	}
}
