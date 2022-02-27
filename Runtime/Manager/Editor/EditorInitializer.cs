namespace SoraCore.Manager.Editor
{
    using MyBox;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EditorInitializer : MonoBehaviour
    {
        [SerializeField] private LevelSO _persistentScene;

        [Separator("Broadcasting on")]
        [SerializeField] private AudioManagerEventChannelSO _audioManagerEC;

        private void Awake() {
            //_persistentScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true)
            //    .Completed += (obj) =>
            //    {
            //        SceneManager.SetActiveScene(obj.Result.Scene);
            //        _audioManagerEC.LoadPlayerPrefsAll();
            //    };
        }
    }
}

