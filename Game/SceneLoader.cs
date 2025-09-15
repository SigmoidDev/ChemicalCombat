using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Cameras;
using Sigmoid.UI;

namespace Sigmoid.Game
{
    /// <summary>
    /// Cleanly handles the additive asynchronous loading of the various scenes
    /// </summary>
	public class SceneLoader : Singleton<SceneLoader>
	{
        private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);

        public event Action<GameScene> OnSceneLoaded;
        public event Action<GameScene> OnSceneUnloading;
        public GameScene CurrentScene { get; private set; }
        public bool IsLoading { get; private set; }

        private void Awake()
        {
            CurrentScene = GameScene.Menu;
            OnSceneLoaded += (scene) => CurrentScene = scene;
        }

        /// <summary>
        /// Unloads whichever of Home, Labyrinth, or Tutorial is laoded
        /// </summary>
        public IEnumerator UnloadEnvironmentAsync()
        {
            IEnumerable<AsyncOperation> operations = new string[]{ "Home", "Labyrinth", "Tutorial" }
            .Where(n => SceneManager.GetSceneByName(n).isLoaded)
            .Select(n => SceneManager.UnloadSceneAsync(n));

            bool isDone = false;
            while(!isDone)
            {
                isDone = true;
                foreach(AsyncOperation operation in operations)
                    if(!operation.isDone) isDone = false;

                yield return null;
            }
        }



        /// <summary>
        /// Reusable IEnumerator for starting the unloading process
        /// </summary>
        /// <returns></returns>
        private IEnumerator PrepareUnloading()
        {
            IsLoading = true;
            MainCamera.FadeOut(0.3f);
            OnSceneUnloading?.Invoke(CurrentScene);
            yield return _waitForSeconds0_3;
        }

        /// <summary>
        /// Reusable method for ending the loading process
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private void FinishLoading(GameScene scene)
        {
            MainCamera.ResetScreenColour();
            MainCamera.CircleReveal(0.6f, 0.2f);
            OnSceneLoaded?.Invoke(scene);
            IsLoading = false;
        }



        /// <summary>
        /// Ends the current game and re-opens the menu
        /// </summary>
        public void ReturnToMenu() => StartCoroutine(ReturnToMenuAsync());
        private IEnumerator ReturnToMenuAsync()
        {
            Time.timeScale = 1f;
            yield return PrepareUnloading();

            yield return UnloadEnvironmentAsync();
            yield return SceneManager.UnloadSceneAsync("Player");
            yield return SceneManager.UnloadSceneAsync("Managers");
            yield return SceneManager.UnloadSceneAsync("Interface");
            yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);

            FinishLoading(GameScene.Menu);
        }



        public void RestartGame() => StartCoroutine(RestartGameAsync());
        private IEnumerator RestartGameAsync()
        {
            Time.timeScale = 1f;
            yield return PrepareUnloading();

            yield return UnloadEnvironmentAsync();
            yield return SceneManager.UnloadSceneAsync("Player");
            yield return SceneManager.UnloadSceneAsync("Managers");
            yield return SceneManager.UnloadSceneAsync("Interface");

            yield return SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Interface", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Home", LoadSceneMode.Additive);

            FinishLoading(GameScene.Home);

            Map.Instance.Redraw(GameScene.Home, null, (size) =>
            {
                Minimap.Instance.UpdateCanvas(size);
                FullscreenMap.Instance.UpdateCanvas(size);
            });
        }



        /// <summary>
        /// Loads all of the required game scenes asynchronously before playing an animation
        /// </summary>
        public void StartGame() => StartCoroutine(StartGameAsync());
        private IEnumerator StartGameAsync()
        {
            yield return PrepareUnloading();

            yield return SceneManager.UnloadSceneAsync("Menu");
            yield return SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Interface", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Home", LoadSceneMode.Additive);

            FinishLoading(GameScene.Home);

            Map.Instance.Redraw(GameScene.Home, null, (size) =>
            {
                Minimap.Instance.UpdateCanvas(size);
                FullscreenMap.Instance.UpdateCanvas(size);
            });
        }



        /// <summary>
        /// Loads all of the required game scenes asynchronously before playing an animation
        /// </summary>
        public void StartTutorial() => StartCoroutine(StartTutorialAsync());
        private IEnumerator StartTutorialAsync()
        {
            yield return PrepareUnloading();

            yield return SceneManager.UnloadSceneAsync("Menu");
            yield return SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Interface", LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);

            Player.Instance.transform.position = new Vector2(-0.5f, 0f);
            FinishLoading(GameScene.Tutorial);

            Map.Instance.Redraw(GameScene.Tutorial, null, (size) =>
            {
                Minimap.Instance.UpdateCanvas(size);
                FullscreenMap.Instance.UpdateCanvas(size);
            });
        }



        /// <summary>
        /// Switches from the Home scene to the Labyrinth, calling all functions required to generate the dungeon
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="size"></param>
        public void EnterLabyrinth(ScriptableFloor floor, ScriptableSize size) => StartCoroutine(EnterLabyrinthAsync(floor, size));
        private IEnumerator EnterLabyrinthAsync(ScriptableFloor floor, ScriptableSize size)
        {
            yield return PrepareUnloading();

            yield return SceneManager.UnloadSceneAsync("Home");
            yield return SceneManager.LoadSceneAsync("Labyrinth", LoadSceneMode.Additive);

            int seed = UnityEngine.Random.Range(0, 100000);
            MapRenderer.Instance.GenerateMap(floor, size, seed, (dungeon) =>
            {
                Player.Instance.transform.position = dungeon.EntrancePosition - 3f * Vector2.right;
                FinishLoading(GameScene.Labyrinth);

                Map.Instance.Redraw(GameScene.Labyrinth, dungeon.Path, (size) =>
                {
                    Minimap.Instance.UpdateCanvas(size);
                    FullscreenMap.Instance.UpdateCanvas(size);
                });
            });
        }

        /// <summary>
        /// In case of a failure to load, retries with a different seed
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="size"></param>
        public void RetryLabyrinth(ScriptableFloor floor, ScriptableSize size) => StartCoroutine(RetryLabyrinthAsync(floor, size));
        private IEnumerator RetryLabyrinthAsync(ScriptableFloor floor, ScriptableSize size)
        {
            yield return PrepareUnloading();

            yield return SceneManager.UnloadSceneAsync("Labyrinth");
            yield return SceneManager.LoadSceneAsync("Labyrinth", LoadSceneMode.Additive);

            int seed = UnityEngine.Random.Range(0, 100000);
            MapRenderer.Instance.GenerateMap(floor, size, seed, (dungeon) =>
            {
                Player.Instance.transform.position = dungeon.EntrancePosition - 3f * Vector2.right;
                FinishLoading(GameScene.Labyrinth);

                Map.Instance.Redraw(GameScene.Labyrinth, dungeon.Path, (size) =>
                {
                    Minimap.Instance.UpdateCanvas(size);
                    FullscreenMap.Instance.UpdateCanvas(size);
                });
            });
        }



        /// <summary>
        /// Exits the Labyrinth scene and returns Home before redrawing the minimap
        /// </summary>
        public void ReturnHome() => StartCoroutine(ReturnHomeAsync());
        private IEnumerator ReturnHomeAsync()
        {
            yield return PrepareUnloading();

            yield return SceneManager.UnloadSceneAsync("Labyrinth");
            yield return SceneManager.LoadSceneAsync("Home", LoadSceneMode.Additive);

            Player.Instance.transform.position = Vector2.up;
            FinishLoading(GameScene.Home);

            Map.Instance.Redraw(GameScene.Home, null, (size) =>
            {
                Minimap.Instance.UpdateCanvas(size);
                FullscreenMap.Instance.UpdateCanvas(size);
            });
        }
	}

    public enum GameScene
    {
        Menu,
        Home,
        Labyrinth,
        Tutorial
    }
}
