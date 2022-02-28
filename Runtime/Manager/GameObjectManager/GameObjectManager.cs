namespace SoraCore.Manager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;

    public class GameObjectManager : SoraManager {
        #region Static -------------------------------------------------------------------------------------------------------
        private static Func<PrefabSO, GameObject> _instantiateRequested;
        /// <summary>
        /// Return an <see cref="GameObject"/> base on <paramref name="pd"/>.
        /// </summary>
        public static GameObject Instantiate(PrefabSO pd) {
            if (_instantiateRequested != null) return _instantiateRequested.Invoke(pd);

            LogWarningForEvent(nameof(GameObjectManager));
            return null;
        }

        private static Action<GameObject> _destroyRequested;
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
            // Return an object from the pool if marked as poolable
            if (TryGetOrCreatePool(pd)) {
                // Return an object from the pool
                ObjectPool<GameObject> pool = _prefabDataToPool[pd];
                GameObject gObj = pool.Get();

#if UNITY_EDITOR
                // Only nested game object in hierarchy when in editor mode
                gObj.transform.parent = _poolToParentTransform[pool];
#endif

                return gObj;
            }

            return Instantiate(pd.prefab);
        }

        private void Release(GameObject gObj) {
            // Does this game object spawned by the system
            if (_instanceIDToPrefabData.TryGetValue(gObj.GetInstanceID(), out PrefabSO pd)) {
                if (TryGetOrCreatePool(pd)) {
                    _prefabDataToPool[pd].Release(gObj);
                    return;
                }
            }
            else {
                SoraCore.LogWarning($"You are trying to destroy an object which are not created by the system", nameof(GameObjectManager), gObj);
            }

            UnityEngine.Object.Destroy(gObj);
        }

        private bool TryGetOrCreatePool(PrefabSO pd) {
            // If marked as poolable
            if (pd.enablePooling) {
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
                    // Add newly created pool to the list
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

            return false;
        }
    }
}