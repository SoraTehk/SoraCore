using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "EC_GameObjectManager", menuName = "SoraCore/GameObject Manager/GameObject Manager EC")]
    public class GameObjectManagerEventChannelSO : ScriptableObject {

        public event Func<PrefabSO, GameObject> InstantiateRequested;
        public event UnityAction<GameObject, PrefabSO> DestroyRequested;

        public GameObject Instantiate(PrefabSO pd) {
            if (InstantiateRequested != null) return InstantiateRequested.Invoke(pd);

            Debug.LogWarning("An game object instantiating event was requested, but no GameObjectManager picked it up.");
            return null;
        }

        public void Destroy(GameObject gObj, PrefabSO pd) {
            if (DestroyRequested != null)
            {
                DestroyRequested.Invoke(gObj, pd);
            }
            else
            {
                Debug.LogWarning("An game object instantiating event was requested, but no GameObjectManager picked it up.");
            }
        }
    }
}
