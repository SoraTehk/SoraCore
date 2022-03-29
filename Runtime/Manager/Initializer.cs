namespace SoraCore.Manager {
    using MyBox;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public class Initializer : MonoBehaviour {
        [SerializeField] private LevelSO _persistentLevel;
        [SerializeField] private LevelSO _levelToLoad;
        
        private void Awake() {
#if UNITY_EDITOR
            Editor.EditorInitializer.IsColdStart = false;
#endif
            _persistentLevel.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnPersistentLevelLoaded;

        }

        private void OnPersistentLevelLoaded(AsyncOperationHandle<SceneInstance> obj) {
            SceneManager.SetActiveScene(obj.Result.Scene);
            SceneManager.UnloadSceneAsync(0);

            //SoundManager.LoadPlayerPrefsAll();
            LevelManager.LoadLevel(_levelToLoad);
        }
    }
}

