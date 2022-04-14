#if UNITY_EDITOR
using SoraCore.Manager.Level;

namespace SoraCore.Manager.EditorTools
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EditorInitializer : MonoBehaviour
    {
        public static bool IsColdStart = true;

        [field: SerializeField] public LevelAsset ThisLevel { get; private set; }
        [field: SerializeField] public bool DoReload { get; private set; }
        [field: SerializeField] public LevelAsset PersistentLevel { get; private set; }

        private async void Awake()
        {
            if (IsColdStart)
            {
                // If the persistent are not already loaded
                if (!SceneManager.GetSceneByName(PersistentLevel.SceneReference.editorAsset.name).isLoaded)
                {
                    // Synchronously loading the persistent scene
                    var op = PersistentLevel.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                    // This line are not actually finished loading the scene
                    op.WaitForCompletion();
                    // So we will have to use await here
                    await op.Task;
                }
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(PersistentLevel.SceneReference.editorAsset.name));

                LevelManager.AddLevelToLoadedList(ThisLevel);
                if (DoReload) LevelManager.LoadLevel(ThisLevel);
            }
        }
    }
}
#endif