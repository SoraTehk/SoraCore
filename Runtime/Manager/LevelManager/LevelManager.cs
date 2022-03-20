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
            _loadedLevels.Enqueue(level);
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
        public static event Action<LoadContext> LoadStarted;
        /// <summary>
        /// Raised when loading progress changed
        /// </summary>
        public static event Action<LoadContext> LoadProgressChanged;
        /// <summary>
        /// Raised when the load sequence finished
        /// </summary>
        public static event Action<LoadContext> LoadFinished;
        #endregion
        #region Static -------------------------------------------------------------------------------------------------------
        private static Action<LevelSO, bool, bool, bool> _loadLevelRequested;

        /// <summary>
        /// Load the level and it sub-levels
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="showLoadingScreen"></param>
        /// <param name="fadeScreen"></param>
        /// <param name="unloadPrevious"></param>
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
        private readonly Queue<LevelSO> _loadedLevels = new();
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

            #region Gather Data
            // Populating levels linked list with recursive function as head first
            var levelsToLoad = GetSubLevels(ld);
            levelsToLoad.AddFirst(ld);

            // Context for event dispatching
            var ctx = new LoadContext()
            {
                LevelsToUnload = unloadPrevious ? _loadedLevels.ToList() : new List<LevelSO>(),
                LevelsToLoad = levelsToLoad.ToList(),
                ShowLoadingScreen = showLoadingScreen,
            };

            // Populate blueprints to unload
            List<BlueprintSO> blueprintsToUnload = unloadPrevious ? GameObjectManager.GetLoadedBlueprints() : new();

            LoadStarted?.Invoke(ctx);
            #endregion

            // *Fading out*

            #region Unload Previous Scenes
            if (unloadPrevious) {
                GameObjectManager.ReleaseAll();

                var unloadOpCount = 0;
                var unloadTasks = new List<Task<SceneInstance>>(_loadedLevels.Count);

                while (_loadedLevels.Count > 0)
                {
                    var sceneRef = _loadedLevels.Dequeue().SceneReference;
                    if(sceneRef.OperationHandle.IsValid()) {
                        // Unload the scene through its AssetReference, i.e. through the Addressable system
                        var op = sceneRef.UnLoadScene();
                        unloadOpCount++;
                        unloadTasks.Add(op.Task);
                    }
#if UNITY_EDITOR
                    else
                    {
                        // After a "cold start", the AsyncOperationHandle has not been used (the scene was already open in the
                        // editor), the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                        SceneManager.UnloadSceneAsync(sceneRef.editorAsset.name);
                    }
#endif
                }

                // Await for all task to complete
                await Task.WhenAll(unloadTasks);
                // Scene mark as unloaded still being updated when unloading so we have to release again for safety
                GameObjectManager.ReleaseAll(); 
            }
            #endregion

            #region Load New Scenes
            int loadOpCount = levelsToLoad.Count;
            var loadTasks = new List<Task<SceneInstance>>(loadOpCount);

            for (int i = 0; i < loadOpCount; i++) {
                // Consume the linked list depend on load type
                LevelSO level = LevelLoadType switch
                {
                    LevelLoadType.Head => levelsToLoad.ConsumeFirst(),
                    LevelLoadType.Tail => levelsToLoad.ConsumeLast(),
                    _ => throw new ArgumentOutOfRangeException(nameof(LevelLoadType), $"Undefined enum value of {nameof(LevelLoadType)}"),
                };

                // Loading the scene
                AssetReference assetRef = level.SceneReference;
                AsyncOperationHandle<SceneInstance> op = assetRef.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
                op.Completed += obj =>
                {
                    _loadedLevels.Enqueue(level);
                    foreach(var bd in level.PreloadBlueprints)
                    {
                        GameObjectManager.Preload(bd);
                        blueprintsToUnload.Remove(bd);
                    }
                };

                // Use task to track progress
                loadTasks.Add(op.Task);            
            }

            // Await for all task to complete
            while (loadTasks.Count > 0) {
                var finishedTask = await Task.WhenAny(loadTasks);
                loadTasks.Remove(finishedTask);

                // Update progress bar
                ctx.MainProgress = (float)(loadOpCount - loadTasks.Count) / loadOpCount;
                LoadProgressChanged?.Invoke(ctx); 
            }

            // Unload blueprints
            foreach (var bd in blueprintsToUnload) GameObjectManager.Clear(bd);

            LoadFinished?.Invoke(ctx);
            #endregion

            // *Fading in*

            _isLoading = false;
        }

        /// <summary>
        /// Produce a <seealso cref="UniqueLinkedList{T}"/> that contain all sub-levels of <paramref name="level"/>.
        /// </summary>
        public static UniqueLinkedList<LevelSO> GetSubLevels(LevelSO level)
        {
            UniqueLinkedList<LevelSO> result = new();
            AddLevelToList(level); // Recursive
            return result;

            #region Local Functions
            void AddLevelToList(LevelSO input)
            {
                // Firstly, add this sub-levels
                foreach (var level in input.SubLevels)
                {
                    result.AddLast(level);
                }
                // Secondly, process it sub-levels
                foreach (var level in input.SubLevels)
                {
                    AddLevelToList(level);
                }
            }
            #endregion
        }

        public static void GetSubLevels(LevelSO level, ref UniqueLinkedList<LevelSO> result)
        {
            AddLevelToList(level, ref result); // Recursive

            #region Local Functions
            static void AddLevelToList(LevelSO input, ref UniqueLinkedList<LevelSO> result)
            {
                // Firstly, add this sub-levels
                foreach (var level in input.SubLevels)
                {
                    result.AddLast(level);
                }
                // Secondly, process it sub-levels
                foreach (var level in input.SubLevels)
                {
                    AddLevelToList(level, ref result);
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