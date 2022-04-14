using SoraCore.Collections;
using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public enum LevelLoadType
    {
        Head,
        Tail
    }

    public partial class LevelManager : SoraManager<LevelManager>
    {
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
        /// <summary>
        /// Load the level and it sub-levels
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="showLoadingScreen"></param>
        /// <param name="fadeScreen"></param>
        /// <param name="unloadPrevious"></param>
        public static void LoadLevel(LevelAsset sd, bool showLoadingScreen = true, bool fadeScreen = true, bool unloadPrevious = true) => GetInstance().InnerLoadLevel(sd, showLoadingScreen, fadeScreen, unloadPrevious);
        #endregion

        [field: SerializeField] public LevelLoadType LevelLoadType { get; private set; } = LevelLoadType.Tail;

        // Parameters for level loading request
        private readonly Queue<LevelAsset> m_LoadedLevels = new();
        private bool m_IsLoading;

        // TODO: Fade screen
        private async void InnerLoadLevel(LevelAsset ld, bool showLoadingScreen, bool fadeScreen, bool unloadPrevious)
        {
            // Prevent race condition
            if (m_IsLoading) return;
            m_IsLoading = true;

            #region Gather Data
            // Populating levels linked list with recursive function as head first
            var levelsToLoad = GetSubLevels(ld);
            levelsToLoad.AddFirst(ld);

            // Context for event dispatching
            var ctx = new LoadContext()
            {
                LevelsToUnload = unloadPrevious ? m_LoadedLevels.ToList() : new List<LevelAsset>(),
                LevelsToLoad = levelsToLoad.ToList(),
                ShowLoadingScreen = showLoadingScreen,
            };

            // Populate blueprints to unload
            List<BlueprintAsset> blueprintsToUnload = unloadPrevious ? InstantiateManager.GetLoadedBlueprints() : new();

            LoadStarted?.Invoke(ctx);
            #endregion

            // *Fading out*

            #region Unload Previous Scenes
            if (unloadPrevious)
            {
                InstantiateManager.ReleaseAll();

                var unloadOpCount = 0;
                var unloadTasks = new List<Task<SceneInstance>>(m_LoadedLevels.Count);

                while (m_LoadedLevels.Count > 0)
                {
                    var sceneRef = m_LoadedLevels.Dequeue().SceneReference;
                    if (sceneRef.OperationHandle.IsValid())
                    {
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
                InstantiateManager.ReleaseAll();
            }
            #endregion

            #region Load New Scenes
            int loadOpCount = levelsToLoad.Count;
            var loadTasks = new List<Task<SceneInstance>>(loadOpCount);

            for (int i = 0; i < loadOpCount; i++)
            {
                // Consume the linked list depend on load type
                LevelAsset level = LevelLoadType switch
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
                    m_LoadedLevels.Enqueue(level);
                    foreach (var bd in level.PreloadBlueprints)
                    {
                        InstantiateManager.Preload(bd);
                        blueprintsToUnload.Remove(bd);
                    }
                };

                // Use task to track progress
                loadTasks.Add(op.Task);
            }

            // Await for all task to complete
            while (loadTasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(loadTasks);
                loadTasks.Remove(finishedTask);

                // Update progress bar
                ctx.MainProgress = (float)(loadOpCount - loadTasks.Count) / loadOpCount;
                LoadProgressChanged?.Invoke(ctx);
            }

            // Unload blueprints
            foreach (var bd in blueprintsToUnload) InstantiateManager.Clear(bd);

            LoadFinished?.Invoke(ctx);
            #endregion

            // *Fading in*

            m_IsLoading = false;
        }

        /// <summary>
        /// Produce a <seealso cref="UniqueLinkedList{T}"/> that contain all sub-levels of <paramref name="level"/>.
        /// </summary>
        public static UniqueLinkedList<LevelAsset> GetSubLevels(LevelAsset level)
        {
            UniqueLinkedList<LevelAsset> result = new();
            AddLevelToList(level); // Recursive
            return result;

            #region Local Functions
            void AddLevelToList(LevelAsset input)
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

        public static void GetSubLevels(LevelAsset level, ref UniqueLinkedList<LevelAsset> result)
        {
            AddLevelToList(level, ref result); // Recursive

            #region Local Functions
            static void AddLevelToList(LevelAsset input, ref UniqueLinkedList<LevelAsset> result)
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

    public struct LoadContext
    {
        public IReadOnlyList<LevelAsset> LevelsToUnload { get; internal set; }
        public IReadOnlyList<LevelAsset> LevelsToLoad { get; internal set; }
        public bool ShowLoadingScreen { get; internal set; }
        public float MainProgress { get; internal set; }
        public float SubProgress { get; internal set; }
    }
}

#if UNITY_EDITOR
namespace SoraCore.Manager.Level
{
    public partial class LevelManager
    {
        #region Static -------------------------------------------------------------------------------------------------------
        /// <summary>
        /// WARNING: EDITOR ONLY METHOD
        /// </summary>
        /// <param name="level"></param>
        public static void AddLevelToLoadedList(LevelAsset level) => GetInstance().InnerAddLevelToLoadedList(level);
        #endregion

        private void InnerAddLevelToLoadedList(LevelAsset level)
        {
            m_LoadedLevels.Enqueue(level);
        }
    }
}
#endif