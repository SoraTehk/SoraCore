namespace SoraCore.Manager.Editor {
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EditorInitializer : MonoBehaviour {
        [SerializeField] private LevelSO _persistentScene;
        private static bool _isColdStart = false;

        private void Awake() {
            if (!SceneManager.GetSceneByName(_persistentScene.sceneReference.editorAsset.name).isLoaded) _isColdStart = true;

            if (_isColdStart) _persistentScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true)
                .Completed += (obj) =>
                {
                    SceneManager.SetActiveScene(obj.Result.Scene);
                };
        }
    }
}

