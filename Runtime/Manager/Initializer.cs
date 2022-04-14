using SoraCore.Manager.Level;

namespace SoraCore.Manager
{
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public class Initializer : MonoBehaviour
    {
        [field: SerializeField] public LevelAsset PersistentLevel { get; private set; }
        [field: SerializeField] public LevelAsset LevelToLoad { get; private set; }

        private void Awake()
        {
#if UNITY_EDITOR
            EditorTools.EditorInitializer.IsColdStart = false;
#endif
            PersistentLevel.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnPersistentLevelLoaded;

        }

        private void OnPersistentLevelLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            SceneManager.SetActiveScene(obj.Result.Scene);
            SceneManager.UnloadSceneAsync(0);

            //SoundManager.LoadPlayerPrefsAll();
            LevelManager.LoadLevel(LevelToLoad);
        }
    }
}

