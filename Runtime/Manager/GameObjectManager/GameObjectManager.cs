namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;
    using UnityEngine.AddressableAssets;

    public class GameObjectManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------

        private static Func<PrefabSO, GameObject> _instantiateRequested;
        private static Action<GameObject> _destroyRequested;
        private static Action<PrefabSO> _preloadRequested;
        private static Action<PrefabSO> _clearRequested;

        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="pd"/>.
        /// </summary>
        public static GameObject Instantiate(PrefabSO pd) {
            if (_instantiateRequested != null) return _instantiateRequested.Invoke(pd);

            LogWarningForEvent(nameof(GameObjectManager));
            return null;
        }

        /// <summary>
        /// Destroy <paramref name="gObj"/> base on it <see cref="PrefabSO"/> settings.
        /// </summary>
        public static void Destroy(GameObject gObj) {
            if (_destroyRequested != null) {
                _destroyRequested.Invoke(gObj);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        /// <summary>
        /// Clear <paramref name="pd"/> pool data.
        /// </summary>
        public static void Clear(PrefabSO pd) {
            if (_clearRequested != null) {
                _clearRequested.Invoke(pd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        /// <summary>
        /// Clear <paramref name="pd"/> pool data.
        /// </summary>
        public static void Preload(PrefabSO pd) {
            if (_preloadRequested != null) {
                _preloadRequested.Invoke(pd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        #endregion

        private readonly Dictionary<GameObject, PrefabSO> _gameObjectToPrefabD = new();
        private readonly Dictionary<PrefabSO, ObjectPool<GameObject>> _prefabDataToPool = new();
#if UNITY_EDITOR
        // Only nested game object in hierarchy when in editor mode
        public Dictionary<ObjectPool<GameObject>, Transform> _poolToParentTransform = new();
#endif

        private void OnEnable() {
            _instantiateRequested += Get;
            _destroyRequested += Release;
            _clearRequested += InnerClear;
            _preloadRequested += InnerPreload;
        }

        private void OnDisable() {
            _instantiateRequested -= Get;
            _destroyRequested -= Release;
            _clearRequested -= InnerClear;
            _preloadRequested -= InnerPreload;
        }
        private GameObject Get(PrefabSO pd) {
            GameObject gObj;

            if (TryGetOrCreatePool(pd)) {
                // Return an object from the pool
                ObjectPool<GameObject> pool = _prefabDataToPool[pd];
                gObj = pool.Get();

#if UNITY_EDITOR
                // Only nested game object in hierarchy when in editor mode
                gObj.transform.parent = _poolToParentTransform[pool];
#endif
            }
            else {
                // Instantiate normal object as it not marked as poolable synchronously
                var op = pd.GameObjectRef.InstantiateAsync();
                op.WaitForCompletion();
                gObj = op.Result;

                _gameObjectToPrefabD[gObj] = pd;
                pd.OnGameObjGet(gObj);
            }

            return gObj;
        }

        private void Release(GameObject gObj) {
            // Does this game object spawned by the system
            if (!_gameObjectToPrefabD.TryGetValue(gObj, out PrefabSO pd)) {
                SoraCore.LogWarning($"You are trying to destroy an object which are not created by the system, no callback will be processed!", nameof(GameObjectManager), gObj);
                UnityEngine.Object.Destroy(gObj);
                return;
            }

            if (TryGetOrCreatePool(pd)) {
                _prefabDataToPool[pd].Release(gObj);
            }
            else {
                pd.OnGameObjRelease(gObj);
                pd.OnGameObjDestroy(gObj);
            }
        }

        private void InnerClear(PrefabSO pd) {
            _prefabDataToPool[pd].Clear();
        }

        private void InnerPreload(PrefabSO obj) {
        }

        private bool TryGetOrCreatePool(PrefabSO pd) {
            // Return false if not poolable
            if (!pd.EnablePooling) return false;

            // If key not found then..
            if (!_prefabDataToPool.ContainsKey(pd)) {
                // ..create pool
                var pool = new ObjectPool<GameObject>(() =>
                {
                    var op = pd.GameObjectRef.InstantiateAsync();
                    op.WaitForCompletion();
                    var gObj = op.Result;

                    _gameObjectToPrefabD[gObj] = pd;

                    return gObj;
                },
                pd.OnGameObjGet,
                pd.OnGameObjRelease,
                gObj =>
                {
                    Addressables.ReleaseInstance(gObj);
                    pd.OnGameObjDestroy(gObj);
                },
                true,
                pd.preload,
                pd.capacity);

                // Add newly created pool to the dict
                _prefabDataToPool[pd] = pool;

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                Transform poolTransformParent = new GameObject("Dynamic").transform;
                poolTransformParent.parent = transform;
                _poolToParentTransform[pool] = poolTransformParent;
#endif
            }

            return true;
        }
    }
}