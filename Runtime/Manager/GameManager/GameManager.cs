namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.AddressableAssets;
    using MyBox;
    using System.Threading.Tasks;
    using SoraCore.Collections;

    using Random = UnityEngine.Random;

    public enum GameState {
        Playing,
        Pausing,
        Loading
    }

    public class GameManager : MonoBehaviour {

        #region Static -------------------------------------------------------------------------------------------------------
        private static event Action<LevelSO, bool, bool, bool> _loadSceneRequested;
        public static void LoadLevel(LevelSO sd, bool showLoadingScreen = true, bool fadeScreen = true, bool unloadPrevious = false) {
            if (_loadSceneRequested != null) {
                _loadSceneRequested.Invoke(sd, showLoadingScreen, fadeScreen, unloadPrevious);
            }
            else {
                Debug.LogWarning("LoadLevel(...) was requested, but no GameManager picked it up.");
            }
        }

        private static event Action<GameState> _changeGameStateRequested;
        public static void ChangeGameState(GameState gameState) {
            if (_changeGameStateRequested != null) {
                _changeGameStateRequested.Invoke(gameState);
            }
            else {
                Debug.LogWarning("ChangeGameState(....) was requested, but no GameManager picked it up.");
            }
        }
        #endregion

        [Range(1, 600)]
        public int TargetFPS = 600;

        [field: SerializeField] public GameState CurrentGameState { get; private set; }
        [field: SerializeField] public float CurrentTimeScale { get; private set; } = 1f;


        // Parameters from scene loading requests
        private readonly UniqueQueue<AssetReference> _scenesToLoad = new();
        private readonly List<AssetReference> _currentlyLoadedScene = new();
        private bool _isLoading;

        private void OnEnable() {
            _loadSceneRequested += InnerLoadLevel;
            _changeGameStateRequested += InnerChangeGameState;
        }

        private void OnDisable() {
            _loadSceneRequested -= InnerLoadLevel;
            _changeGameStateRequested -= InnerChangeGameState;

        }


        #region GameState ----------------------------------------------------------------------------------------------------
        private void InnerChangeGameState(GameState newGameState) {
            if (CurrentGameState == newGameState) return;

            switch (newGameState) {
                case GameState.Pausing: PauseGame(); break;
                case GameState.Playing or GameState.Loading: ResumeGame(); break;
                default: throw new ArgumentOutOfRangeException(nameof(newGameState), $"Not expected GameState value: {newGameState}");
            }

            CurrentGameState = newGameState;
        }

        private void PauseGame() {
            // Store current timescale for resuming
            CurrentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        private void ResumeGame() {
            //Resuming timescale
            Time.timeScale = CurrentTimeScale;
            AudioListener.pause = false;
        }
        #endregion


        // TODO: Loading screen, fade screen, adjust input map, callback when finished
        private async void InnerLoadLevel(LevelSO ld, bool showLoadingScreen, bool fadeScreen, bool unloadPrevious) {
            // Prevent race condition
            if (_isLoading) return;
            _isLoading = true;

            // *Fading out, adjust input*

            UIManager.ShowScreen(UIType.Load, showLoadingScreen);

            // Populating levels queue with recursive function as head first
            _scenesToLoad.Enqueue(ld.sceneReference);
            AddLevelToQueue(ld);

            // TODO: Fix pooling system on unload
            if (unloadPrevious) {
                if (_currentlyLoadedScene.Count > 0) {
                    foreach (var sceneRef in _currentlyLoadedScene) {
                        if (sceneRef.OperationHandle.IsValid()) {
                            sceneRef.UnLoadScene();
                        }
#if UNITY_EDITOR
                        else {
                            // After a "cold start", the AsyncOperationHandle has not been used (the scene was already open in the editor),
                            // the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                            //SceneManager.UnloadSceneAsync(sceneRef.editorAsset.name);
                        }
#endif
                    }
                }
            }

            // Loading scene
            int loadingOperationCount = _scenesToLoad.Count;
            Task<SceneInstance>[] tasks = new Task<SceneInstance>[loadingOperationCount];
            for (int i = 0; i < loadingOperationCount; i++) {
                // UNDONE: Removeable
                await Task.Delay(500);

                // Dequeue & start loading scene asynchronously
                AssetReference assetRef = _scenesToLoad.Dequeue();
                AsyncOperationHandle<SceneInstance> operation = assetRef.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
                tasks[i] = operation.Task;

                // Add scene to loaded list
                operation.Completed += obj => _currentlyLoadedScene.Add(assetRef);
                
                // Update progress bar
                float mainProgress = (float)(i + 1) / loadingOperationCount;
                float subProgress = Random.Range(0f, 1f);
                UIManager.UpdateLoadScreen(mainProgress, subProgress);
            }

            // Await for all task to complete
            while (loadingOperationCount > 0) {
                Task finishedTask = await Task.WhenAny(tasks);
                loadingOperationCount--;
            }

            UIManager.ShowScreen(UIType.Load, false);
            
            // *Fading in, adjust input*

            _isLoading = false;

            #region Local Functions
            void AddLevelToQueue(LevelSO input) {
                foreach (var level in input.subLevels) {
                    _scenesToLoad.Enqueue(level.sceneReference);
                }

                foreach (var level in input.subLevels) {
                    AddLevelToQueue(level);
                }
            }
            #endregion
        }
    }
}