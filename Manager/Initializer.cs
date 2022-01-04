using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using MyBox;

namespace SoraCore.Manager {
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private SceneSO _persistentScene;
        [SerializeField] private SceneSO _sceneToLoad;

        [Separator("Broadcasting on")]
        [SerializeField] private AudioManagerEventChannelSO _audioManagerEC;
        [SerializeField] private LoadSceneEventChannelSO _loadSceneEC;

        private void Awake() {
            _persistentScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnPersistentSceneCompleted;
        }

        private void OnPersistentSceneCompleted(AsyncOperationHandle<SceneInstance> obj) {
            SceneManager.SetActiveScene(obj.Result.Scene);
            // Unload initialization scene (the only scene in build index)
            SceneManager.UnloadSceneAsync(0);

            _audioManagerEC.LoadPlayerPrefsAll();

            _loadSceneEC.Raise(_sceneToLoad);
        }
    }
}

