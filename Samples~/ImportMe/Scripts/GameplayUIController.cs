using MyBox;
using SoraCore.Manager;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIController : MonoBehaviour
{
    [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }

    [SerializeField] private LevelSO _menuLevel;

    private Button _menuButton;
    private Button _saveButton;
    private Button _loadButton;
    private TextField _fileNameField;
    private Toggle _spawnerToggle;

    private GameObject _spawner;

    private void Update()
    {
        if (!_spawner) _spawner = GameObject.FindGameObjectWithTag("Spawner");
    }

    private void OnEnable()
    {
        _menuButton = Document.rootVisualElement.Q<Button>("menu-button");
        _menuButton.clicked += OnMenuButtonClicked;

        _saveButton = Document.rootVisualElement.Q<Button>("save-button");
        _saveButton.clicked += OnSaveButtonClicked;

        _loadButton = Document.rootVisualElement.Q<Button>("load-button");
        _loadButton.clicked += OnLoadButtonClicked;

        _fileNameField = Document.rootVisualElement.Q<TextField>("file-name-field");

        _spawnerToggle = Document.rootVisualElement.Q<Toggle>("spawner-toggle");
        _spawnerToggle.value = true;
        _spawnerToggle.RegisterCallback<ChangeEvent<bool>>(SpawnToggleCallback);
    }

    private void OnDisable()
    {
        _menuButton.clicked -= OnMenuButtonClicked;
        _saveButton.clicked -= OnSaveButtonClicked;
        _loadButton.clicked -= OnLoadButtonClicked;
    }

    private void OnMenuButtonClicked()
    {
        LevelManager.LoadLevel(_menuLevel, true, true, true);
    }

    private void OnSaveButtonClicked()
    {
        DataManager.Save(_fileNameField.text);
    }

    private void OnLoadButtonClicked()
    {
        DataManager.Load(_fileNameField.text);
    }

    private void SpawnToggleCallback(ChangeEvent<bool> evt)
    {
        _spawner.SetActive(evt.newValue);
    }
}
