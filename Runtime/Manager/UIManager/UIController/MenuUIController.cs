namespace SoraCore.Manager {
    using MyBox;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MenuUIController : MonoBehaviour {
        [SerializeField] LevelSO _ingameLevel;
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

        [SerializeField] private Button _startButton;

        private void Awake() {
            _startButton = Document.rootVisualElement.Q<Button>("start-button");
            _startButton.clicked += () =>
            {
                UIManager.ShowScreen(UIType.Menu, false);
                LevelManager.LoadLevel(_ingameLevel, true, true, true);
            };
        }

        private void OnDisable() {
            Debug.Log("OnDisable: " + _startButton.text);
        }
    }
}
