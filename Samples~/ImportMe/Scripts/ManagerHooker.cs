using SoraCore.Manager.Audio;
using SoraCore.Manager.Level;
using UnityEngine;

public class ManagerHooker : MonoBehaviour
{
    [field: SerializeField] public MixerGroupAsset MixerGroup { get; private set; }
    [field: SerializeField] public LoadingUIController LoadingUIController { get; private set; }

    private void OnEnable()
    {
        LevelManager.LoadStarted += OnLoadStarted;
        LevelManager.LoadProgressChanged += OnLoadProgressChanged;
        LevelManager.LoadFinished += OnLoadFinished;

        AudioManager.VolumeChanged += (grp, value) =>
        {
            PlayerPrefs.SetFloat($"{nameof(AudioManager)}_{grp.VolumeParameter}", value);
            PlayerPrefs.Save();
        };
    }

    private void OnDisable()
    {
        LevelManager.LoadStarted -= OnLoadStarted;
        LevelManager.LoadProgressChanged -= OnLoadProgressChanged;
        LevelManager.LoadFinished -= OnLoadFinished;
    }

    private void ChangeVolume()
    {
        AudioManager.SetVolume(MixerGroup, Random.Range(0f, 1f));
    }
    private void OnLoadStarted(LoadContext ctx)
    {
        LoadingUIController.enabled = ctx.ShowLoadingScreen;
    }
    private void OnLoadProgressChanged(LoadContext ctx)
    {
        LoadingUIController.MainProgress = ctx.MainProgress;
        LoadingUIController.SubProgress = ctx.SubProgress;
    }
    private void OnLoadFinished(LoadContext ctx)
    {
        LoadingUIController.enabled = false;
    }
}