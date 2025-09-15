using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using UnityEngine;
using Sigmoid.Game;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Singleton for instantiating furniture GameObjects and collections within rooms
    /// </summary>
	public class FurniturePlacer : Singleton<FurniturePlacer>
	{
		[SerializeField] private LayerMask groundMask;
        [SerializeField] private AirlockDoor airlockDoorFront;
        [SerializeField] private AirlockDoor airlockDoorSide;
        [SerializeField] private Transform doorParent;

        /// <summary>
        /// Loops from the sides to the middle of a room and places furniture along the top and bottom
        /// </summary>
        /// <param name="room"></param>
        /// <param name="offset"></param>
        /// <param name="drawDebugRays"></param>
        /// <returns></returns>
        public IEnumerator PlaceFurniture(Room room, Vector2 offset)
		{
			float start = room.bounds.xMin + offset.x + 2f;
			float end = room.bounds.xMax + offset.x - 2f;

			float mid = room.bounds.center.x + offset.x;
			float half = room.bounds.width * 0.5f;
			float refY = room.bounds.center.y + offset.y;
			float height = room.bounds.height * 0.5f;

			System.Func<float, bool> leftCondition = x => x <= mid;
			System.Func<float, bool> rightCondition = x => x >= mid;

			yield return PlaceAlongLine(start, refY, leftCondition, mid, half, 1f, height, Vector2.up, room.physicalRoom.transform);
			yield return PlaceAlongLine(end, refY, rightCondition, mid, half, -1f, height, Vector2.up, room.physicalRoom.transform);
			yield return PlaceAlongLine(start, refY, leftCondition, mid, half, 1f, height, -Vector2.up, room.physicalRoom.transform);
			yield return PlaceAlongLine(end, refY, rightCondition, mid, half, -1f, height, -Vector2.up, room.physicalRoom.transform);
		}

        /// <summary>
        /// Places furniture in a line going from one side to the middle
        /// </summary>
        /// <param name="start"></param>
        /// <param name="y"></param>
        /// <param name="condition"></param>
        /// <param name="mid"></param>
        /// <param name="half"></param>
        /// <param name="movement"></param>
        /// <param name="height"></param>
        /// <param name="castDir"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
		public IEnumerator PlaceAlongLine(float start, float y, System.Func<float, bool> condition, float mid, float half, float movement, float height, Vector2 castDir, Transform parent)
		{
			yield return null;

			float x = start;
			Furniture previous = null;

			int thingsAttempted = 0;
			while(thingsAttempted < 10 && condition(x))
			{
				thingsAttempted++;

				float distanceFactor = Mathf.Abs(x - mid) / half + 0.5f;
				Furniture furniture = ChooseFurniture(distanceFactor, previous, castDir == Vector2.up);
				if(furniture == null) continue;

				x += 0.5f * furniture.size.x * movement;

				RaycastHit2D raycast = Physics2D.CapsuleCast(new Vector2(x, y), furniture.size, CapsuleDirection2D.Horizontal, 0f, castDir, height, groundMask);
				if(raycast.collider != null)
				{
					if(raycast.collider.gameObject.layer == LayerMask.NameToLayer("Furniture"))
					{
						x -= 0.5f * furniture.size.x * movement;
						continue;
					}
					else if(raycast.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
					{
						Vector2 spawnPos = raycast.point - 0.5f * castDir;
						RaycastHit2D checkRight = Physics2D.Raycast(spawnPos, Vector2.right * movement, 1f * furniture.size.x, groundMask);
						if(checkRight.collider != null) continue;

						float offsetX = x + movement * furniture.size.x * 0.4f;
						RaycastHit2D checkDepth = Physics2D.CapsuleCast(new Vector2(offsetX, y), furniture.size, CapsuleDirection2D.Horizontal, 0f, castDir, height, groundMask);
						if(Mathf.Abs(raycast.distance - checkDepth.distance) > 0.2f) continue;

						GameObject obj = Instantiate(furniture.prefab, spawnPos, Quaternion.identity, parent);
						previous = furniture;

                        if(furniture.autoFlipX)
                        {
                            bool shouldFlip = movement > 0;
                            obj.GetComponent<SpriteRenderer>().flipX = shouldFlip;
                            obj.GetComponentInChildren<SpriteRenderer>().flipX = shouldFlip;
                        }
					}
				}

				x += 0.5f * furniture.size.x * movement;
			}
		}

        /// <summary>
        /// Chooses a random Furniture item based on a weighted random algorithm and a few biases
        /// </summary>
        /// <param name="distanceFactor"></param>
        /// <param name="previousFurniture"></param>
        /// <param name="isUpwards"></param>
        /// <returns></returns>
		private Furniture ChooseFurniture(float distanceFactor, Furniture previousFurniture, bool isUpwards)
		{
			float totalWeight = 0f;
			List<WeightedFurniture> newWeights = new List<WeightedFurniture>();

			foreach(WeightedFurniture weighting in FloorManager.Instance.Floor.furnitureList)
			{
				bool allowedOnBottom = weighting.furniture.allowedOnBottom;
				if(!isUpwards && !allowedOnBottom) continue;

				float cornerFactor = Mathf.Pow(weighting.furniture.cornerPreference + 1, distanceFactor);
				if(allowedOnBottom && !isUpwards && cornerFactor < 2.3f) continue;

				float similarityFactor = previousFurniture == weighting.furniture ? (weighting.furniture.similarityPreference + 0.5f) : 1f;
				float overallWeighting = weighting.weight * cornerFactor * similarityFactor;

				newWeights.Add(new WeightedFurniture(overallWeighting, weighting.furniture));
				totalWeight += overallWeighting;
			}

			float randomValue = Random.value * totalWeight;
			foreach(WeightedFurniture weighting in newWeights)
			{
				randomValue -= weighting.weight;
				if(randomValue <= 0f) return weighting.furniture;
			}

			return null;
		}

        /// <summary>
        /// Places preset collections of furniture in a room based on its size
        /// </summary>
        /// <param name="room"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
		public IEnumerator PlaceLayouts(Room room, Vector2 offset)
		{
            if(FloorManager.Instance.Floor.layoutList.Count == 0) yield break;
			if(room.SizeCategory == RoomSize.Small) yield break;

			int numLayouts = (int) room.SizeCategory;
			int overallAttempts = 0;
			List<Rect> placedRects = new List<Rect>();

			for(int i = 0; i < numLayouts && overallAttempts < numLayouts * 2; i++)
			{
				int random = Random.Range(0, FloorManager.Instance.Floor.layoutList.Count);
				Layout layout = FloorManager.Instance.Floor.layoutList[random];

				if(layout.FitsWithin(room.bounds))
				{
					int positionAttempts = 0;
					Rect? acceptedRect = null;

					while(positionAttempts < 6 && acceptedRect == null)
					{
						positionAttempts++;

						Vector2 origin = RandomPointWithin(room.bounds, layout.size);
						Rect rect = new Rect(origin.x, origin.y, layout.size.x, layout.size.y);

						bool overlaps = false;
						foreach(Rect other in placedRects)
						{
							if(rect.Overlaps(other))
							{
								overlaps = true;
								break;
							}
						}

						if(!overlaps)
						{
							placedRects.Add(rect);
							acceptedRect = rect;
						}
					}

					if(acceptedRect == null)
					{
						overallAttempts++;
						i--;
						continue;
					}

					Vector2 centralPoint = acceptedRect.Value.position + 0.5f * layout.size + offset;
					Instantiate(layout.prefab, centralPoint, Quaternion.identity, room.physicalRoom.transform);
				}
			}
		}

        /// <summary>
        /// Generates a random point within a rect
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="size"></param>
        /// <returns></returns>
		private Vector2 RandomPointWithin(RectInt bounds, Vector2 size)
		{
			float x = Random.Range(bounds.xMin + 3, bounds.xMax - size.x - 3);
			float y = Random.Range(bounds.yMin + 4, bounds.yMax - size.y - 5);
			return new Vector2(x, y);
		}

        /// <summary>
        /// Instantiates an airlock door at the given position
        /// </summary>
        /// <returns></returns>
        public AirlockDoor CreateDoor(Vector2 position, bool isVertical) => Instantiate(isVertical ? airlockDoorFront : airlockDoorSide, position, Quaternion.identity, doorParent);
    }

    /// <summary>
    /// A furniture item and its spawn weightings, to be used in weighted random
    /// </summary>
	[System.Serializable]
	public class WeightedFurniture
	{
        /// <summary>
        /// A string which automatically updates to the name of the furniture prefab (doesn't affect gameplay, only the Unity Editor)
        /// </summary>
		[HideInInspector] public string name;
		public void UpdateName() => name = (furniture == null || furniture.prefab == null) ? "New Furniture" : furniture.prefab.name;

		[Min(0)] public float weight;
		public Furniture furniture;

		public WeightedFurniture(float weight, Furniture furniture)
		{
			this.weight = weight;
			this.furniture = furniture;
		}
	}

    /// <summary>
    /// A furniture prefab and set of preferences for its spawn conditions
    /// </summary>
	[System.Serializable]
	public class Furniture
	{
        /// <summary>
        /// The actual object that should be instantiated
        /// </summary>
		public GameObject prefab;

        /// <summary>
        /// The amount of physical space that this piece of furniture takes up
        /// </summary>
		[Min(0f)] public Vector2 size;

        /// <summary>
        /// How much this furniture prefers to be placed towards the sides/corners of rooms
        /// </summary>
		[Range(0f, 1f)] public float cornerPreference;

        /// <summary>
        /// How much this furniture prefers to be placed near others of the same type
        /// </summary>
		[Range(0f, 1f)] public float similarityPreference;

        /// <summary>
        /// Should the FurniturePlacer be allowed to place this along the bottom side, or only the top
        /// </summary>
		public bool allowedOnBottom;

        /// <summary>
        /// Whether or not the sprite should flip horizontally to face the centre of the room
        /// </summary>
        public bool autoFlipX;
	}

    /// <summary>
    /// A group of furniture items and decorations that are placed together
    /// </summary>
	[System.Serializable]
	public class Layout
	{
        /// <summary>
        /// A string which automatically updates to the name of the layout prefab (doesn't affect gameplay, only the Unity Editor)
        /// </summary>
		[HideInInspector] public string name;
		public void UpdateName() => name = prefab == null ? "New Layout" : prefab.name;

		public GameObject prefab;
		[Min(0f)] public Vector2 size;

        /// <summary>
        /// Returns whether or not this specific layout fits within the bounds of a room
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
		public bool FitsWithin(RectInt bounds)
		{
			return size.x <= bounds.size.x - 5
			&& size.y <= bounds.size.y - 6;
		}
	}
}
