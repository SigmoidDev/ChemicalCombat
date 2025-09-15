using UnityEngine.Rendering.Universal;
using UnityEngine;
using Sigmoid.Utilities;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Used to generate the Light2Ds at the corner of every room
    /// </summary>
	public class LightingPlacer : Singleton<LightingPlacer>
	{
        [SerializeField] private Light2D lightPrefab;

        /// <summary>
        /// Adds lights to the four corners of a room, or only to the top if the room is tiny
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="room"></param>
		public void AddLights(Dungeon dungeon, Room room)
		{
			Quaternion topLeftRotation = Quaternion.Euler(new Vector3(0f, 0f, 225f));
			Light2D topLeftLight = Instantiate(lightPrefab, room.TopLeft + dungeon.RenderOffset, topLeftRotation, transform);
			topLeftLight.pointLightOuterRadius = room.Size * 1f;

			Quaternion topRightRotation = Quaternion.Euler(new Vector3(0f, 0f, 135f));
			Light2D topRightLight = Instantiate(lightPrefab, room.TopRight + dungeon.RenderOffset, topRightRotation, transform);
			topRightLight.pointLightOuterRadius = room.Size * 1f;

			float distanceFactor = MathsHelper.DistanceFalloff(room.bounds.center.x, dungeon.Size.x * 2.5f, 1.1f);
			topLeftLight.intensity *= distanceFactor;
			topRightLight.intensity *= distanceFactor;

			//We don't need bottom lights for tiny rooms
			if(room.Size > 12)
			{
				Quaternion bottomLeftRotation = Quaternion.Euler(new Vector3(0f, 0f, 315f));
				Light2D bottomLeftLight = Instantiate(lightPrefab, room.BottomLeft + dungeon.RenderOffset, bottomLeftRotation, transform);
				bottomLeftLight.pointLightOuterRadius = room.Size;

				Quaternion bottomRightRotation = Quaternion.Euler(new Vector3(0f, 0f, 45f));
				Light2D bottomRightLight = Instantiate(lightPrefab, room.BottomRight + dungeon.RenderOffset, bottomRightRotation, transform);
				bottomRightLight.pointLightOuterRadius = room.Size;

				bottomLeftLight.intensity *= distanceFactor;
				bottomRightLight.intensity *= distanceFactor;
			}
			else
			{
				topLeftLight.pointLightOuterRadius *= 1.5f;
				topRightLight.pointLightOuterRadius *= 1.5f;
			}
		}
	}
}
