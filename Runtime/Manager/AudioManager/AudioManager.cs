using SoraCore.Manager.Instantiate;

namespace SoraCore.Manager.Audio
{
    using MyBox;
    using System;
    using UnityEngine;
    using UnityEngine.Audio;

    public partial class AudioManager : SoraManager<AudioManager>
    {
        #region Dispatching Static Event -------------------------------------------------------------------------------------
        /// <summary>
        /// Raised when the mixer group volume changed
        /// </summary>
        public static event Action<MixerGroupAsset, float> VolumeChanged;
        #endregion

        #region Static -------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Instantiate an <see cref="AudioSource"/> that play an audio clip from <paramref name="ad"/> at <paramref name="pos"/> using embedded configuration
        /// </summary>
        public static void Play(AudioCueAsset ad, Vector3 pos) => Play(ad, pos, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Instantiate an <see cref="AudioSource"/> that play an audio clip from <paramref name="ad"/> at <paramref name="pos"/> using custom configuration
        /// </summary>
        public static void Play(AudioCueAsset ad, Vector3 pos, AudioConfigurationAsset config, MixerGroupAsset group)
        {
            GetInstance().InnerPlayPos(ad, pos, config, group);
        }

        /// <summary>
        /// Attach an <see cref="AudioSource"/> to <paramref name="parent"/> that play an audio clip from <paramref name="ad"/> using embedded configuration
        /// </summary>
        public static void Play(AudioCueAsset ad, Transform parent) => Play(ad, parent, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Attach an <see cref="AudioSource"/> to <paramref name="parent"/> that play an audio clip from <paramref name="ad"/> using custom configuration
        /// </summary>
        public static void Play(AudioCueAsset ad, Transform parent, AudioConfigurationAsset config, MixerGroupAsset group)
        {
            GetInstance().InnerPlayTransform(ad, parent, config, group);
        }

        /// <summary>
        /// Request music source to play <paramref name="ad"/> using embedded configuration
        /// </summary>
        public static void PlayMusic(AudioCueAsset ad) => PlayMusic(ad, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Request music source to play <paramref name="ad"/> using custom configuration
        /// </summary>
        public static void PlayMusic(AudioCueAsset ad, AudioConfigurationAsset config, MixerGroupAsset group)
        {
            GetInstance().InnerPlayMusic(ad, config, group);
        }

        /// <summary>
        /// Change dB volume of <paramref name="mixerGroup"/> base on <paramref name="value"/>
        /// </summary>
        public static void SetVolume(MixerGroupAsset group, float value)
        {
            GetInstance().InnerSetVolume(group, value);
        }
        #endregion

        public const float MinVolume = 0.0001f;
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }
        [field: SerializeField] public BlueprintAsset AudioSourceBlueprint { get; private set; }
        [field: SerializeField] public float VolumeMultiplier { get; private set; } = 30f;
        [field: SerializeField] public AudioSource MusicSource { get; private set; }

        private void InnerPlayPos(AudioCueAsset ad, Vector3 pos, AudioConfigurationAsset cfg, MixerGroupAsset grp)
        {
            // Spawn an audio source at target position
            AudioSource audioSource = InstantiateManager.Get(AudioSourceBlueprint).GetComponent<AudioSource>();
            audioSource.transform.position = pos;

            SetUpAndPlay(audioSource, ad, cfg, grp);
        }

        private void InnerPlayTransform(AudioCueAsset ad, Transform parent, AudioConfigurationAsset cfg, MixerGroupAsset grp)
        {
            // Spawn an audio source at target position
            AudioSource audioSource = InstantiateManager.Get(AudioSourceBlueprint).GetComponent<AudioSource>();
            audioSource.transform.parent = parent;

            SetUpAndPlay(audioSource, ad, cfg, grp);
        }

        private void InnerPlayMusic(AudioCueAsset ad, AudioConfigurationAsset cfg, MixerGroupAsset grp) => SetUpAndPlay(MusicSource, ad, cfg, grp);

        private void SetUpAndPlay(AudioSource source, AudioCueAsset ad, AudioConfigurationAsset cfg, MixerGroupAsset grp)
        {
            // Apply configuration to the audio source
            source.clip = ad.GetClip;
            source.loop = ad.Loop;
            source.ApplyConfig(cfg);
            source.outputAudioMixerGroup = grp.Group;

            // Play
            source.Play();
        }

        public void InnerSetVolume(MixerGroupAsset grp, float value)
        {
            if (value > 1)
            {
                string s1 = $"{grp.Group.name} value".Bold();
                SoraCore.LogWarning($"{s1} parameter > 1, it could be too loud.", nameof(AudioManager));
            }

            // Magic number https://www.youtube.com/watch?v=MmWLK9sN3s8&t=374s (6:14)
            float dBValue = Mathf.Log10(Mathf.Max(MinVolume, value)) * VolumeMultiplier;
            AudioMixer.SetFloat(grp.VolumeParameter, dBValue);

            VolumeChanged?.Invoke(grp, value);

            return;
        }
    }
}