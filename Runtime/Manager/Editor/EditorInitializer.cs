using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using MyBox;

namespace SoraCore.Manager {
    public class EditorInitializer : MonoBehaviour
    {
        [SerializeField] private SceneSO _persistentScene;

        [Separator("Broadcasting on")]
        [SerializeField] private AudioManagerEventChannelSO _audioManagerEC;

        private void Awake() {
            _persistentScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true)
                .Completed += (obj) => SceneManager.SetActiveScene(obj.Result.Scene);
            _audioManagerEC.LoadPlayerPrefsAll();
        }
    }
}

