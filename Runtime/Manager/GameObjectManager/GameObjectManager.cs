namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Pool;

    public class GameObjectManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------
        private static Func<BlueprintSO, GameObject> _getRequested;
        private static Action<BlueprintSO> _releaseRequested;
        private static Action _releaseAllRequested;
        private static Action<GameObject> _releaseInstanceRequested;
        private static Action<BlueprintSO> _preloadRequested;
        private static Action<BlueprintSO> _clearRequested;
        private static Action _clearAllRequested;
        private static Func<List<BlueprintSO>> _getLoadedBlueprintsRequested;

        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="bd"/>.
        /// </summary>
        public static GameObject Get(BlueprintSO bd) {
            if (_getRequested != null) return _getRequested.Invoke(bd);

            LogWarningForEvent(nameof(GameObjectManager));
            return null;
        }
        /// <summary>
        /// Release instance(s) of <paramref name="bd"/> pool.
        /// </summary>
        public static void Release(BlueprintSO bd)
        {
            if (_releaseRequested != null)
            {
                _releaseRequested.Invoke(bd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Release instance(s) of all pools.
        /// </summary>
        public static void ReleaseAll()
        {
            if (_releaseAllRequested != null)
            {
                _releaseAllRequested.Invoke();
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Release <paramref name="gObj"/> base on it <see cref="BlueprintSO"/> settings.
        /// </summary>
        public static void ReleaseInstance(GameObject gObj) {
            if (_releaseInstanceRequested != null) {
                _releaseInstanceRequested.Invoke(gObj);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Preload instance(s) for <paramref name="bd"/> pool.
        /// </summary>
        public static void Preload(BlueprintSO bd) {
            if (_preloadRequested != null) {
                _preloadRequested.Invoke(bd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Destroy instance(s) of <paramref name="bd"/> pool.
        /// </summary>
        public static void Clear(BlueprintSO bd) {
            if (_clearRequested != null) {
                _clearRequested.Invoke(bd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Destroy instance(s) and pool(s).
        /// </summary>
        public static void ClearAll()
        {
            if (_clearAllRequested != null)
            {
                _clearAllRequested.Invoke();
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }
        /// <summary>
        /// Return all active blueprints
        /// </summary>
        public static List<BlueprintSO> GetLoadedBlueprints()
        {
            if (_getLoadedBlueprintsRequested != null) return _getLoadedBlueprintsRequested.Invoke();

            LogWarningForEvent(nameof(GameObjectManager));
            return null;
        }
        #endregion

        [field: SerializeField] public BlueprintCallbackSO DefaultGetCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackSO DefaultReleaseCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackSO DefaultDestroyCallback { get; private set; }

        private readonly Dictionary<GameObject, BlueprintSO> _gameObjToBlueprint = new();
        private readonly Dictionary<BlueprintSO, List<GameObject>> _blueprintToActiveGameObjs = new();
        private readonly Dictionary<BlueprintSO, ObjectPool<GameObject>> _blueprintToPool = new();
#if UNITY_EDITOR
        // Only nested game object in hierarchy when in editor mode
        private readonly Dictionary<ObjectPool<GameObject>, Transform> _poolToParentTransform = new();
#endif

        private void OnEnable() {
            _getRequested += InnerGet;
            _releaseRequested += InnerRelease;
            _releaseAllRequested += InnerReleaseAll;
            _releaseInstanceRequested += InnerReleaseInstance;
            _preloadRequested += InnerPreload;
            _clearRequested += InnerClear;
            _clearAllRequested += InnerClearAll;
            _getLoadedBlueprintsRequested += InnerGetLoadedBlueprints;
        }

        private void OnDisable() {
            _getRequested -= InnerGet;
            _releaseRequested -= InnerRelease;
            _releaseAllRequested -= InnerReleaseAll;
            _releaseInstanceRequested -= InnerReleaseInstance;
            _preloadRequested -= InnerPreload;
            _clearRequested -= InnerClear;
            _clearAllRequested -= InnerClearAll;
            _getLoadedBlueprintsRequested -= InnerGetLoadedBlueprints;
        }

        private GameObject InnerGet(BlueprintSO bd) {
             GameObject gObj;

            if (CreatePoolIfCan(bd)) {
                // Return an object from the pool
                ObjectPool<GameObject> pool = _blueprintToPool[bd];
                gObj = pool.Get();

#if UNITY_EDITOR
                // Only nested game object in hierarchy when in editor mode
                gObj.transform.parent = _poolToParentTransform[pool];
#endif
            }
            else {
                // Instantiate new game object as it not marked as poolable
                gObj = Instantiate(bd.Prefab);

                // Save for later look up
                _gameObjToBlueprint[gObj] = bd;

#pragma warning disable UNT0007
                // Invoke get callback
                (bd.GetCallback ?? DefaultGetCallback).Invoke(gObj); // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
#pragma warning restore UNT0007
            }

            if (!_blueprintToActiveGameObjs.ContainsKey(bd)) _blueprintToActiveGameObjs[bd] = new List<GameObject>();
            _blueprintToActiveGameObjs[bd].Add(gObj);

            return gObj;
        }

        private void InnerRelease(BlueprintSO bd)
        {
            foreach(var gObj in _blueprintToActiveGameObjs[bd]) InnerReleaseInstance(gObj, false);
            _blueprintToActiveGameObjs[bd].Clear();
        }

        private void InnerReleaseAll() {
            foreach (var blueprint in InnerGetLoadedBlueprints()) InnerRelease(blueprint);
        }

        private void InnerReleaseInstance(GameObject gObj) {
            if (!gObj)
            {
                SoraCore.LogWarning($"You are trying to release null!", nameof(GameObjectManager));
                return;
            }

            InnerReleaseInstance(gObj, true);
        }
        private void InnerReleaseInstance(GameObject gObj, bool removeFromActiveList)
        {
            // Does this game object spawned by the system
            if (!_gameObjToBlueprint.TryGetValue(gObj, out BlueprintSO bd))
            {
                SoraCore.LogWarning($"You are trying to release an object which are not created by the system, no callback will be processed!", nameof(GameObjectManager), gObj);
                Destroy(gObj);
                return;
            }

            if (CreatePoolIfCan(bd))
            {
                _blueprintToPool[bd].Release(gObj);
            }
            else
            {
                // Callback
#pragma warning disable UNT0007
                (bd.ReleaseCallback ?? DefaultReleaseCallback).Invoke(gObj); // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
                (bd.DestroyCallback ?? DefaultDestroyCallback).Invoke(gObj); // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
#pragma warning restore UNT0007
            }

            if (removeFromActiveList) _blueprintToActiveGameObjs[bd].Remove(gObj);
        }

        private void InnerPreload(BlueprintSO bd) {
            if (!CreatePoolIfCan(bd)) return;
            if (_blueprintToPool[bd].CountAll >= bd.preload) return;

            Debug.Log($"{bd.name} preloaded");


            // OPTIMIZABLE: InnerPreload()
            var gObjs = new GameObject[bd.preload];
            for (int i = 0; i < bd.preload; i++) gObjs[i] = InnerGet(bd);
            for (int i = 0; i < bd.preload; i++) InnerReleaseInstance(gObjs[i]);
        }

        private void InnerClear(BlueprintSO bd)
        {
            Debug.Log($"{bd.name} cleared");
            InnerRelease(bd);
            _blueprintToPool[bd].Clear();
        }

        private void InnerClearAll()
        {
            foreach (var blueprint in InnerGetLoadedBlueprints()) InnerClear(blueprint);
            _gameObjToBlueprint.Clear();
        }

        private List<BlueprintSO> InnerGetLoadedBlueprints() => _gameObjToBlueprint.Values.Distinct().ToList();

        private bool CreatePoolIfCan(BlueprintSO bd) {
            // Return false if not poolable
            if (!bd.EnablePooling) return false;

            // If key not found then..
            if (!_blueprintToPool.ContainsKey(bd)) {
                // ..create pool
                var pool = new ObjectPool<GameObject>(() =>
                {
                    var gObj = Instantiate(bd.Prefab);

                    _gameObjToBlueprint[gObj] = bd;

                    return gObj;
                },
#pragma warning disable UNT0007
                (bd.GetCallback ?? DefaultGetCallback).Invoke, // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
                (bd.ReleaseCallback ?? DefaultReleaseCallback).Invoke, // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
                (bd.DestroyCallback ?? DefaultDestroyCallback).Invoke, // Ignore warning 'UNT0007' since this not a runtime instantiate/destroy field
#pragma warning restore UNT0007
                true,
                bd.preload,
                bd.capacity);

                // Add newly created pool to the dict
                _blueprintToPool[bd] = pool;

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                Transform poolTransformParent = new GameObject(bd.name).transform;
                poolTransformParent.parent = transform;
                _poolToParentTransform[pool] = poolTransformParent;
#endif
            }

            return true;
        }
    }
}