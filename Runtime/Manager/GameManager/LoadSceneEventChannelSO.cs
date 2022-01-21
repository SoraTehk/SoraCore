using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "EC_LoadScene", menuName = "SoraCore/Game Manager/Load Scene EC")]
    public class LoadSceneEventChannelSO : ScriptableObject {

        public event UnityAction<SceneSO, bool, bool> Requested;

        public void Raise(SceneSO sd, bool showLoadingScreen = false, bool fadeScreen = false) {
            if (Requested != null)
            {
                Requested.Invoke(sd, showLoadingScreen, fadeScreen);
            }
            else
            {
                Debug.LogWarning("A scene loading event was requested, but no GameManager picked it up.");
            }
        }
    }
}
