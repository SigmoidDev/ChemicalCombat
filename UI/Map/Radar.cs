using System.Collections.Generic;
using Sigmoid.Generation;
using Sigmoid.Upgrading;
using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Game;
using UnityEngine;

namespace Sigmoid.UI
{
	public class Radar : MonoBehaviour
	{
		[SerializeField] private RectTransform playerTracker;
        [SerializeField] private TrackerPool trackerPool;
        [SerializeField] private MarkerPool markerPool;

        private List<Tracker> activeTrackers;
        private List<Marker> activeMarkers;

        private void Awake()
        {
            //both unsubscribed manually in OnDestroy
            SceneLoader.Instance.OnSceneUnloading += RemoveAll;
            SceneLoader.Instance.OnSceneLoaded += RegisterEvents;
            SceneLoader.Instance.OnSceneLoaded += AddMarkers;

            activeTrackers = new List<Tracker>();
            activeMarkers = new List<Marker>();
        }

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneUnloading -= RemoveAll;
            SceneLoader.Instance.OnSceneLoaded -= RegisterEvents;
        }

        /// <summary>
        /// Registers the scene-specific events that will unsubscribe automatically with said scene
        /// </summary>
        /// <param name="scene"></param>
        private void RegisterEvents(GameScene scene)
        {
            if(scene == GameScene.Menu) return;
            EnemySpawner.Instance.OnEnemySpawned += OnSpawn;
        }

        private void Update() => playerTracker.anchoredPosition = (Vector2) Player.Instance.transform.position + Map.Instance.Offset;

        /// <summary>
        /// Creates a tracker when an enemy spawns
        /// </summary>
        /// <param name="enemy"></param>
        private void OnSpawn(Enemy enemy)
        {
            if(!Perks.Has(Perk.Radar)) return;
            activeTrackers.Add(trackerPool.Fetch().Initialise(trackerPool, enemy));
        }

        /// <summary>
        /// Loops over all rooms and creates markers
        /// </summary>
        /// <param name="room"></param>
        private void AddMarkers(GameScene scene)
        {
            if(scene != GameScene.Labyrinth || !Perks.Has(Perk.Cartographer)) return;

            foreach(Room room in MapRenderer.Instance.Dungeon.Rooms)
            {
                Vector2 centre = room.overrideCentre != null ? room.overrideCentre.Value : room.bounds.center;
                Vector2 position = centre + room.dungeonReference.RenderOffset;
                Sprite sprite = markerPool.GetSprite(room.type);

                activeMarkers.Add(markerPool.Fetch().Initialise(position, sprite));
            }
        }

        /// <summary>
        /// Removes all active markers and trackers from the map on scene unload
        /// </summary>
        /// <param name="scene"></param>
        private void RemoveAll(GameScene scene)
        {
            foreach(Tracker tracker in activeTrackers)
                trackerPool.Release(tracker);
            activeTrackers = new List<Tracker>();

            foreach(Marker marker in activeMarkers)
                markerPool.Release(marker);
            activeMarkers = new List<Marker>();
        }
	}
}
