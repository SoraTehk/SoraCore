namespace SoraCore.Manager {
    using Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using Random = UnityEngine.Random;
#if UNITY_EDITOR
    public partial class LevelManager {
        #region Static -------------------------------------------------------------------------------------------------------

        private static Action<LevelSO> _addLevelToLoadedListRequested;
        /// <summary>
        /// WARNING: EDITOR ONLY METHOD
        /// </summary>
        /// <param name="level"></param>
        public static void AddLevelToLoadedList(LevelSO level) {
            if (_addLevelToLoadedListRequested != null) {
                _addLevelToLoadedListRequested.Invoke(level);
                return;
            }

            LogWarningForEvent(nameof(LevelManager));
        }

        #endregion

        private void InnerAddLevelToLoadedList(LevelSO level) {
            _loadedLevels.Add(level);
        }
    }
#endif

    public enum LevelLoadType {
        Head,
        Tail
    }

    public partial class LevelManager : SoraManager {
        #region Dispatching Static Event -------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the load sequence started
        /// </summary>
        public static event Action<LoadContext> OnLoadStarted;
        /// <summary>
        /// Raised when loading progress changed
        /// </summary>
        public static event Action<LoadContext> OnLoadProgressChanged;
        /// <summary>
        /// Raised when the load sequence finished
        /// </summary>
        public static event Action<LoadContext> OnLoadFinished;

        #endregion
        #region Static -------------------------------------------------------------------------------------------------------

        private static Action<LevelSO, bool, bool, bool> _loadLevelRequested;

        public static void LoadLevel(LevelSO sd, bool showLoadingScreen = true, bool fadeScreen = true, bool unloadPrevious = true) {
            if (_loadLevelRequested != null) {
                _loadLevelRequested.Invoke(sd, showLoadingScreen, fadeScreen, unloadPrevious);
                return;
            }

            LogWarningForEvent(nameof(LevelManager));
        }

        #endregion
        
        [field: SerializeField] public LevelLoadType LevelLoadType { get; private set; } = LevelLoadType.Tail;

        // Parameters for level loading request
        private readonly List<LevelSO> _loadedLevels = new();
        private readonly List<AssetReference> _loadedScenes = new();
        private readonly UniqueLinkedList<LevelSO> _levelsToLoad = new();
        private bool _isLoading;

        private void OnEnable() {
            _loadLevelRequested += InnerLoadLevel;
#if UNITY_EDITOR
            _addLevelToLoadedListRequested += InnerAddLevelToLoadedList;
#endif
        }

        private void OnDisable() {
            _loadLevelRequested -= InnerLoadLevel;
#if UNITY_EDITOR
            _addLevelToLoadedListRequested -= InnerAddLevelToLoadedList;
#endif
        }

        // TODO: Fade screen
        private async void InnerLoadLevel(LevelSO ld, bool showLoadingScreen, bool fadeScreen, bool unloadPrevious) {
            // Prevent race condition
            if (_isLoading) return;
            _isLoading = true;

            // Populating levels linked list with recursive function as head first
            _levelsToLoad.AddLast(ld);
            AddLevelToList(ld);
            
            // Context for event dispatching
            LoadContext ctx = new()
            {
                LevelsToUnload = unloadPrevious ? _loadedLevels.ToList() : new List<LevelSO>(),
                LevelsToLoad = _levelsToLoad.ToList(),
                ShowLoadingScreen = showLoadingScreen,
            };
            OnLoadStarted?.Invoke(ctx);

            // *Fading out*

            // Unloading previous loaded scenes
            if (unloadPrevious && _loadedLevels.Count > 0) {
                foreach (var level in _loadedLevels) {
                    AssetReference sceneRef = level.SceneReference;
                    if (sceneRef.OperationHandle.IsValid()) {
                        // Unload the scene through its AssetReference, i.e. through the Addressable system
                        sceneRef.UnLoadScene();
                        // REFACTOR: Do we need to unload handle too?
                    }
#if UNITY_EDITOR
                    else {
                        // After a "cold start", the AsyncOperationHandle has not been used (the scene was already open in the
                        // editor), the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                        SceneManager.UnloadSceneAsync(sceneRef.editorAsset.name);
                    }
#endif
                }

                _loadedLevels.Clear();
            }

            // Loading new scenes
            int totalOpCount = _levelsToLoad.Count;
            List<Task<SceneInstance>> tasks = new(totalOpCount);

            for (int i = 0; i < totalOpCount; i++) {
                // Consume the linked list depend on load type
                LevelSO level = LevelLoadType switch
                {
                    LevelLoadType.Head => _levelsToLoad.ConsumeFirst(),
                    LevelLoadType.Tail => _levelsToLoad.ConsumeLast(),
                    _ => throw new ArgumentOutOfRangeException(nameof(LevelLoadType), $"Undefined enum value of {nameof(LevelLoadType)}"),
                };

                // Loading the scene
                AssetReference assetRef = level.SceneReference;
                AsyncOperationHandle<SceneInstance> op = assetRef.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
                op.Completed += obj => _loadedLevels.Add(level);

                // Use task to track progress
                tasks.Add(op.Task);            
            }

            // Await for all task to complete
            while (tasks.Any()) {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);

                // Update progress bar
                ctx.MainProgress = (float)(totalOpCount - tasks.Count) / totalOpCount;
                OnLoadProgressChanged?.Invoke(ctx); 
            }

            OnLoadFinished?.Invoke(ctx);

            // *Fading in*

            _isLoading = false;

            #region Local Functions

            void AddLevelToList(LevelSO input) {
                // Firstly, add this sub-levels
                foreach (var level in input.SubLevels) {
                    _levelsToLoad.AddLast(level);
                }
                // Secondly, process it sub-levels
                foreach (var level in input.SubLevels) {
                    AddLevelToList(level);
                }
            }

            #endregion
        }
    }

    public struct LoadContext {
        public IReadOnlyList<LevelSO> LevelsToUnload { get; internal set; }
        public IReadOnlyList<LevelSO> LevelsToLoad { get; internal set; }
        public bool ShowLoadingScreen { get; internal set; }
        public float MainProgress { get; internal set; }
        public float SubProgress { get; internal set; }
    }
}