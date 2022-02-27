namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using MyBox;

    public class Initializer : MonoBehaviour
    {
        [SerializeField] private LevelSO _persistentLevel;
        [SerializeField] private LevelSO _levelToLoad;

        [Separator("Broadcasting on")]
        [SerializeField] private AudioManagerEventChannelSO _audioManagerEC;

        private void Awake() {
            _persistentLevel.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnPersistentLevelLoaded;
        }

        private void OnPersistentLevelLoaded(AsyncOperationHandle<SceneInstance> obj) {
            SceneManager.SetActiveScene(obj.Result.Scene);
            SceneManager.UnloadSceneAsync(0);

            //_audioManagerEC.LoadPlayerPrefsAll();
            GameManager.LoadLevel(_levelToLoad);
        }
    }
}

