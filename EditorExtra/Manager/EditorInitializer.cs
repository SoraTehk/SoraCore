#if UNITY_EDITOR
namespace SoraCore.Manager.Editor {
    using UnityEngine;
    using UnityEngine.SceneManagement;

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
                var op = _persistentScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                // This line are not actually finished loading the scene?         
                op.WaitForCompletion();

                // So we will have to use await here
                await op.Task;

                SceneManager.SetActiveScene(op.Result.Scene);

                LevelManager.AddLevelToLoadedList(_thisScene);
                if (_reload) LevelManager.LoadLevel(_thisScene);
            }
        }
    }
}
#endif