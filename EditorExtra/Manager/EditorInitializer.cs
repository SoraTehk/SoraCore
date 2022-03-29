#if UNITY_EDITOR
namespace SoraCore.Manager.Editor
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EditorInitializer : MonoBehaviour
    {
        public static bool IsColdStart = true;

        [SerializeField] private LevelSO _thisLevel;
        [SerializeField] private bool _reload;
        [SerializeField] private LevelSO _persistentLevel;

        private async void Awake()
        {
            if (IsColdStart)
            {
                // If the persistent are not already loaded
                if (!SceneManager.GetSceneByName(_persistentLevel.SceneReference.editorAsset.name).isLoaded)
                {
                    // Synchronously loading the persistent scene
                    var op = _persistentLevel.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                    // This line are not actually finished loading the scene
                    op.WaitForCompletion();
                    // So we will have to use await here
                    await op.Task;
                }
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_persistentLevel.SceneReference.editorAsset.name));

                LevelManager.AddLevelToLoadedList(_thisLevel);
                if (_reload) LevelManager.LoadLevel(_thisLevel);
            }
        }
    }
}
#endif