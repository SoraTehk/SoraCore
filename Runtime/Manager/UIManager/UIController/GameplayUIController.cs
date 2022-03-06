namespace SoraCore.Manager {
    using MyBox;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class GameplayUIController : MonoBehaviour {
        [SerializeField] LevelSO _menuLevel;
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

        private Button _menuButton;
        private Button _saveButton;
        private Button _loadButton;
        private TextField _fileNameField;
        private Toggle _spawnerToggle;

        private GameObject _spawner;

        private void Awake() {
            _menuButton = Document.rootVisualElement.Q<Button>("menu-button");
            _menuButton.clicked += () =>
            {
                UIManager.ShowScreen(UIType.Gameplay, false);
                LevelManager.LoadLevel(_menuLevel, true, true, true);
            };

            _saveButton = Document.rootVisualElement.Q<Button>("save-button");
            _saveButton.clicked += () =>
            {
                DataManager.Save(_fileNameField.text);
            };

            _loadButton = Document.rootVisualElement.Q<Button>("load-button");
            _loadButton.clicked += () =>
            {
                DataManager.Load(_fileNameField.text);
            };

            _fileNameField = Document.rootVisualElement.Q<TextField>("file-name-field");

            _spawnerToggle = Document.rootVisualElement.Q<Toggle>("spawner-toggle");
            _spawnerToggle.RegisterCallback<ChangeEvent<bool>>(SpawnToggleCallback);
        }

        private void OnDisable() {
            Debug.Log("OnDisable: " + _fileNameField.text);
        }

        private void Update() {
            if(!_spawner) _spawner = GameObject.FindGameObjectWithTag("Spawner");
        }

        private void SpawnToggleCallback(ChangeEvent<bool> evt) {
            _spawner.SetActive(evt.newValue);
        }

        
    }
}