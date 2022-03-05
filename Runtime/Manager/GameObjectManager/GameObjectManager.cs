namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;

    public class GameObjectManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------

        private static Func<PrefabSO, GameObject> _instantiateRequested;
        private static Action<GameObject> _destroyRequested;

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

        #endregion

        private readonly Dictionary<int, PrefabSO> _instanceIDToPrefabData = new();
        private readonly Dictionary<PrefabSO, ObjectPool<GameObject>> _prefabDataToPool = new();
#if UNITY_EDITOR
        // Only nested game object in hierarchy when in editor mode
        public Dictionary<ObjectPool<GameObject>, Transform> _poolToParentTransform = new();
#endif

        private void OnEnable() {
            _instantiateRequested += Get;
            _destroyRequested += Release;
        }

        private void OnDisable() {
            _instantiateRequested -= Get;
            _destroyRequested -= Release;
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
                // Instantiate normal object as it not marked as poolable
                gObj = Instantiate(pd.prefab);
                _instanceIDToPrefabData[gObj.GetInstanceID()] = pd;
                pd.OnGameObjGet(gObj);
            }

            return gObj;
        }

        private void Release(GameObject gObj) {
            // Does this game object spawned by the system
            if (!_instanceIDToPrefabData.TryGetValue(gObj.GetInstanceID(), out PrefabSO pd)) {
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

        private bool TryGetOrCreatePool(PrefabSO pd) {
            // Return false if not poolable
            if (!pd.enablePooling) return false;

            // If key not found then..
            if (!_prefabDataToPool.ContainsKey(pd)) {
                // ..create pool
                var pool = new ObjectPool<GameObject>(() =>
                {
                    var instance = Instantiate(pd.prefab);
                    _instanceIDToPrefabData[instance.GetInstanceID()] = pd;

                    return instance;
                },
                    pd.OnGameObjGet,
                    pd.OnGameObjRelease,
                    pd.OnGameObjDestroy,
                    true,
                    pd.preload,
                    pd.capacity);

                // Add newly created pool to the dict
                _prefabDataToPool[pd] = pool;

                // Only nested game object in hierarchy when in editor mode
#if UNITY_EDITOR
                Transform poolTransformParent = new GameObject(pd.prefab.name).transform;
                poolTransformParent.parent = transform;
                _poolToParentTransform[pool] = poolTransformParent;
#endif
            }

            return true;
        }
    }
}