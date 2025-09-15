using System.Collections;
using System;
using Sigmoid.Utilities;
using Sigmoid.Rooms;
using Sigmoid.Game;
using UnityEngine.Tilemaps;
using UnityEngine;
using NavMeshPlus.Components;

namespace Sigmoid.Generation
{
    /// <summary>
    /// Singleton for creating the dungeon upon entering a floor
    /// </summary>
	public class MapRenderer : Singleton<MapRenderer>
	{
        public Dungeon Dungeon { get; private set; }

		[field: SerializeField] public Tilemap Walls { get; private set; }
		[field: SerializeField] public Tilemap Floors { get; private set; }
		[field: SerializeField] public NavMeshSurface NavMesh1 { get; private set; }
		[field: SerializeField] public NavMeshSurface NavMesh2 { get; private set; }

        [field: SerializeField] public ScriptableArena EntranceRoom { get; private set; }
        [field: SerializeField] public ScriptableArena ExitRoom { get; private set; }

        /// <summary>
        /// Called when a PhysicalRoom is instantiated from a Room
        /// </summary>
        public event Action<Room> OnRoomCreated;

        /// <summary>
        /// Performs all necessary steps to run BSP and generate the rooms, entrances and exits, to pathfind start to finish, and to add furniture and lighting
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="size"></param>
        /// <param name="seed"></param>
        /// <param name="callback"></param>
		public void GenerateMap(ScriptableFloor floor, ScriptableSize size, int seed, Action<Dungeon> callback)
		{
			Dungeon = DungeonGenerator.GenerateDungeon(size.size, size.padding, seed, size.depth, 2, 0, 1, 5, 4);
			try{ Dungeon.CreateEntrance().BlockEntrance().CreateExit().CreateExitRoom(floor).CreateCorridors(); } //idk how to properly fix this so i'll just use a try catch block
            catch(Exception e)
            {
                if(e is InvalidDungeonException) //literally just retry with a new seed and pray to god that it doesn't fail twice
                {
                    Debug.LogError($"Map failed to generate! Seed {seed} on floor {floor} with size {size}");
                    SceneLoader.Instance.RetryLabyrinth(floor, size);
                    return;
                }
            }
			FillTilemap();

            //TODO: Add CreateBossRoom(floor) after CreateExit before CreateExitRoom

            Dungeon.Path = RoomTraverser.FindPath(Dungeon.FirstRoom, Dungeon.ExitRoom);
            if(Dungeon.Path == null)
            {
                Debug.LogError($"Map failed to generate! Seed {seed} on floor {floor} with size {size}");
                SceneLoader.Instance.RetryLabyrinth(floor, size);
                return;
            }
            Dungeon = RoomAssigner.AssignTypes(Dungeon, seed);

            Dungeon.Entrance.type = RoomType.Portal;
            //Dungeon.BossRoom.type = RoomType.Boss;
            Dungeon.ExitRoom.type = RoomType.Portal;

			foreach(Room room in Dungeon.Rooms)
			{
				RoomAssigner.Instance.GenerateRoom(room.bounds.center + Dungeon.RenderOffset, room.type).Initialise(room);
                if(room.type.IsStandard() || room.type == RoomType.Boss) LightingPlacer.Instance.AddLights(Dungeon, room);
                OnRoomCreated?.Invoke(room);
			}

			StartCoroutine(PopulateOnceTilemapRenders());
			IEnumerator PopulateOnceTilemapRenders()
			{
				yield return new WaitForFixedUpdate();
				foreach(Room room in Dungeon.Rooms)
                {
                    if(room.type.IsCustom()) continue;

					yield return FurniturePlacer.Instance.PlaceFurniture(room, Dungeon.RenderOffset);
                    if(room.type == RoomType.Enemy) yield return FurniturePlacer.Instance.PlaceLayouts(room, Dungeon.RenderOffset);
                }

				yield return new WaitForFixedUpdate();
                Walls.CompressBounds();
                Floors.CompressBounds();

				yield return new WaitForFixedUpdate();
				NavMesh1.BuildNavMeshAsync();
				NavMesh2.BuildNavMeshAsync();

                callback?.Invoke(Dungeon);
			}
		}



		/// <summary>
		/// Populates the tilemap based on the dungeon's map
		/// </summary>
		public void FillTilemap()
		{
            ScriptableFloor floor = FloorManager.Instance.Floor;
			int width = Dungeon.PaddedSize.x;
			int height = Dungeon.PaddedSize.y;

			Vector3Int[] positions = new Vector3Int[width * height];
			TileBase[] wallTiles = new TileBase[width * height];
			TileBase[] floorTiles = new TileBase[width * height];

			int i = 0;
			for(int x = 0; x < width; x++)
			{
				for(int y = 0; y < height; y++)
				{
					positions[i] = new Vector3Int(x, y, 0);
					wallTiles[i] = Dungeon.Map[x, y] ? null : floor.wallTile;
					floorTiles[i] = floor.floorTile;
					i++;
				}
			}

			Walls.SetTiles(positions, wallTiles);
			Floors.SetTiles(positions, floorTiles);
		}
	}

    public class InvalidDungeonException : Exception
    {

    }
}
