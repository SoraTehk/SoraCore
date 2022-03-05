#if UNITY_EDITOR
namespace SoraCore.Manager.Editor {
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;

    public class EditorInitializer : MonoBehaviour {
        [SerializeField] private LevelSO _thisScene;
        [SerializeField] private bool _reload;
        [SerializeField] private LevelSO _persistentScene;
        private bool _isColdStart = false;

        private async void Awake() {
            // If the persistent are not already loaded
            if (!SceneManager.GetSceneByName(_persistentScene.SceneReference.editorAsset.name).isLoaded) _isColdStart = true;

            if (_isColdStart) {
                // Synchronously loading the persistent scene
                AsyncOperationHandle<SceneInstance> op = _persistentScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                // op.Completed += (completedOp) => SceneManager.SetActiveScene(completedOp.Result.Scene);
                op.WaitForCompletion();

                await op.Task;

                SceneManager.SetActiveScene(op.Result.Scene);

                LevelManager.AddLevelToLoadedList(_thisScene);
                if (_reload) LevelManager.LoadLevel(_thisScene);
            }
        }


    }
}
#endif