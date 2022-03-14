namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;
    using UnityEngine.AddressableAssets;

    public class GameObjectManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------

        private static Func<BlueprintSO, GameObject> _getRequested;
        private static Action<GameObject> _releaseRequested;
        private static Action<BlueprintSO> _preloadRequested;
        private static Action<BlueprintSO> _clearRequested;

        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="bd"/>.
        /// </summary>
        public static GameObject Get(BlueprintSO bd) {
            if (_getRequested != null) return _getRequested.Invoke(bd);

            LogWarningForEvent(nameof(GameObjectManager));
            return null;
        }

        /// <summary>
        /// Destroy <paramref name="gObj"/> base on it <see cref="BlueprintSO"/> settings.
        /// </summary>
        public static void Release(GameObject gObj) {
            if (_releaseRequested != null) {
                _releaseRequested.Invoke(gObj);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        /// <summary>
        /// Clear <paramref name="bd"/> pool data.
        /// </summary>
        public static void Clear(BlueprintSO bd) {
            if (_clearRequested != null) {
                _clearRequested.Invoke(bd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        /// <summary>
        /// Clear <paramref name="bd"/> pool data.
        /// </summary>
        public static void Preload(BlueprintSO bd) {
            if (_preloadRequested != null) {
                _preloadRequested.Invoke(bd);
                return;
            }

            LogWarningForEvent(nameof(GameObjectManager));
        }

        #endregion

        private readonly Dictionary<GameObject, BlueprintSO> _gameObjectToBlueprint = new();
        private readonly Dictionary<BlueprintSO, ObjectPool<GameObject>> _blueprintToPool = new();
#if UNITY_EDITOR
        // Only nested game object in hierarchy when in editor mode
        public Dictionary<ObjectPool<GameObject>, Transform> _poolToParentTransform = new();
#endif

        private void OnEnable() {
            _getRequested += InnerGet;
            _releaseRequested += InnerRelease;
            _clearRequested += InnerClear;
            _preloadRequested += InnerPreload;
        }

        private void OnDisable() {
            _getRequested -= InnerGet;
            _releaseRequested -= InnerRelease;
            _clearRequested -= InnerClear;
            _preloadRequested -= InnerPreload;
        }
        private GameObject InnerGet(BlueprintSO bd) {
            GameObject gObj;

            if (TryGetOrCreatePool(bd)) {
                // Return an object from the pool
                ObjectPool<GameObject> pool = _blueprintToPool[bd];
                gObj = pool.Get();

#if UNITY_EDITOR
                // Only nested game object in hierarchy when in editor mode
                gObj.transform.parent = _poolToParentTransform[pool];
#endif
            }
            else {
                // Instantiate normal object as it not marked as poolable synchronously
                var op = bd.AssetRef.InstantiateAsync();
                op.WaitForCompletion();
                gObj = op.Result;

                _gameObjectToBlueprint[gObj] = bd;
                bd.OnGameObjGet(gObj);
            }

            return gObj;
        }

        private void InnerRelease(GameObject gObj) {
            if (!gObj)
            {
                SoraCore.LogWarning($"You are trying to release null!", nameof(GameObjectManager));
                return;
            }

            // Does this game object spawned by the system
            if (!_gameObjectToBlueprint.TryGetValue(gObj, out BlueprintSO bd)) {
                SoraCore.LogWarning($"You are trying to release an object which are not created by the system, no callback will be processed!", nameof(GameObjectManager), gObj);
                Destroy(gObj);
                return;
            }

            if (TryGetOrCreatePool(bd)) {
                _blueprintToPool[bd].Release(gObj);
            }
            else {
                bd.OnGameObjRelease(gObj);
                bd.OnGameObjDestroy(gObj);
            }
        }

        private void InnerClear(BlueprintSO bd) {
            _blueprintToPool[bd].Clear();
        }

        private void InnerPreload(BlueprintSO bd) {
            bd.AssetRef.InstantiateAsync().Completed += op => Addressables.Release(op);
        }

        private bool TryGetOrCreatePool(BlueprintSO bd) {
            // Return false if not poolable
            if (!bd.EnablePooling) return false;

            // If key not found then..
            if (!_blueprintToPool.ContainsKey(bd)) {
                // ..create pool
                var pool = new ObjectPool<GameObject>(() =>
                {
                    var op = bd.AssetRef.InstantiateAsync();
                    op.WaitForCompletion();
                    var gObj = op.Result;

                    _gameObjectToBlueprint[gObj] = bd;

                    return gObj;
                },
                bd.OnGameObjGet,
                bd.OnGameObjRelease,
                gObj =>
                {
                    Addressables.ReleaseInstance(gObj);
                    bd.OnGameObjDestroy(gObj);
                },
                true,
                bd.preload,
                bd.capacity);

                // Add newly created pool to the dict
                _blueprintToPool[bd] = pool;

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                Transform poolTransformParent = new GameObject(bd.AssetRef.editorAsset.name).transform;
                poolTransformParent.parent = transform;
                _poolToParentTransform[pool] = poolTransformParent;
#endif
            }

            return true;
        }
    }
}