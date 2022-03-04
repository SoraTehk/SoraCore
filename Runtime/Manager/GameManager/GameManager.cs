using SoraCore.Collections;

namespace SoraCore.Manager {
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;

    using Random = UnityEngine.Random;

    public enum GameState {
        Playing,
        Pausing,
        Loading
    }

    public enum LevelLoadType {
        Head,
        Tail
    }

    public class GameManager : SoraManager {

        #region Static -------------------------------------------------------------------------------------------------------
        private static Action<LevelSO, bool, bool, bool> _loadSceneRequested;
        public static void LoadLevel(LevelSO sd, bool showLoadingScreen = true, bool fadeScreen = true, bool unloadPrevious = false) {
            if (_loadSceneRequested != null) {
                _loadSceneRequested.Invoke(sd, showLoadingScreen, fadeScreen, unloadPrevious);
                return;
            }

            LogWarningForEvent(nameof(GameManager));
        }

        private static Action<GameState> _changeGameStateRequested;
        public static void ChangeGameState(GameState gameState) {
            if (_changeGameStateRequested != null) {
                _changeGameStateRequested.Invoke(gameState);
                return;
            }

            LogWarningForEvent(nameof(GameManager));
        }
        #endregion

        [field: SerializeField] public LevelLoadType LevelLoadType { get; private set; } = LevelLoadType.Tail;

        [Range(1, 600)]
        public int TargetFPS = 600;

        [field: SerializeField] public GameState CurrentGameState { get; private set; }
        [field: SerializeField] public float CurrentTimeScale { get; private set; } = 1f;


        // Parameters from scene loading requests
        private readonly UniqueLinkedList<AssetReference> _scenesToLoad = new();
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


        // TODO: Fade screen, adjust input map, callback when finished
        private async void InnerLoadLevel(LevelSO ld, bool showLoadingScreen, bool fadeScreen, bool unloadPrevious) {
            // Prevent race condition
            if (_isLoading) return;
            _isLoading = true;

            // *Fading out, adjust input*

            UIManager.ShowScreen(UIType.Load, showLoadingScreen);

            // Populating levels linked list with recursive function as head first
            _scenesToLoad.AddLast(ld.sceneReference);

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

                // Consume the linked list depend on load type
                AssetReference assetRef = LevelLoadType switch
                {
                    LevelLoadType.Head => _scenesToLoad.ConsumeFirst(),
                    LevelLoadType.Tail => _scenesToLoad.ConsumeLast(),
                    _ => throw new ArgumentOutOfRangeException(nameof(LevelLoadType), $"Undefined enum value of {nameof(LevelLoadType)}"),
                };

                // Loading the scene
                AsyncOperationHandle<SceneInstance> operation = assetRef.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
                operation.Completed += obj => _currentlyLoadedScene.Add(assetRef);

                // Use task to track
                tasks[i] = operation.Task;

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
                    _scenesToLoad.AddLast(level.sceneReference);
                }

                foreach (var level in input.subLevels) {
                    AddLevelToQueue(level);
                }
            }
            #endregion
        }
    }
}