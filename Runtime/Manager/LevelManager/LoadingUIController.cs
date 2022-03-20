namespace SoraCore.Manager {
    using MyBox;
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class LoadingUIController : MonoBehaviour {
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

        [Range(0f, 1f)] public float MainProgress;
        [Range(0f, 1f)] public float SubProgress;

        [SerializeField] private ProgressBar _mainProgressBar;
        [SerializeField] private ProgressBar _subProgressBar;

        private void Awake() {
            _mainProgressBar = Document.rootVisualElement.Q<ProgressBar>("main-progress");
            _subProgressBar = Document.rootVisualElement.Q<ProgressBar>("sub-progress");
        }

        private void Update() {
            // TODO: Smoother progress bar
            _mainProgressBar.value = MainProgress;
            _subProgressBar.value = SubProgress;
        }
    }
}
