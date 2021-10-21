using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sora;
using Sora.Extension;

namespace Sora.Manager {
    public class LoadingCanvas : MonoBehaviour
    {
        [SerializeField] private Image _progressBarImage;
        public Image ProgressBarImage {
            get => _progressBarImage;
        }

        void Awake() {
            transform.Find("ProgressBar").GetComponentNullCheck<Image>(ref _progressBarImage, transform);
        }
    }
}