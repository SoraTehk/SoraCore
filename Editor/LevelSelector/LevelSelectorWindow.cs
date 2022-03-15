using SoraCore.Collections;
using SoraCore.Manager;

namespace SoraCore.EditorTools {
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public partial class LevelSelectorWindow : EditorWindow, IHasCustomMenu {
        #region Static ----------------------------------------------------------------------------------------------------
        [MenuItem("Tools/SoraCore/Level Selector")]
        public static void ShowWindow() {
            GetWindow<LevelSelectorWindow>("Level Selector");
        }
        #endregion

        public VisualTreeAsset VisibleListEntryUXML;
        public VisualTreeAsset LevelListUXML;
        public VisualTreeAsset LevelListEntryUXML;

        private List<LevelContext> _levelContexts
        {
            get => _data._levelContexts;
            set => _data._levelContexts = value;
        }

        private ListView _listView;
        private List<LevelSO> _visibleLevels;
        private LevelListWindow _levelListWindow;

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open Level List"), false, () => _levelListWindow = LevelListWindow.ShowWindow(this));
        }

        private void OnEnable()
        {
            EditorPrefsKey = $"{Application.companyName}.{Application.productName}.SoraCore.EditorTools.LevelSelector";
            LoadData();
        }

        private void OnDisable()
        {
            if (_levelListWindow != null) _levelListWindow.Close();
            SaveData();
        }

        private void CreateGUI() {
            _listView = new();
            rootVisualElement.Add(_listView);

            SetupListView();
        }

        private void SetupListView()
        {
            _listView.makeItem = () =>
            {
                TemplateContainer entry = VisibleListEntryUXML.Instantiate();
                entry.userData = new VisibleLevelListEntryController(entry);
                return entry;
            };
            _listView.bindItem = (item, index) =>
            {
                var ctrl = item.userData as VisibleLevelListEntryController;

                // Object field
                ctrl.LevelSOField.value = _visibleLevels[index];

                // Single button: Unload everything then load only this (no sub-levels)
                ctrl.SingleButton.clicked += () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        string path = AssetDatabase.GUIDToAssetPath(_visibleLevels[index].SceneReference.AssetGUID);
                        EditorSceneManager.OpenScene(path);
                    }
                };

                // Full button: Unload everything then load this level and it sub-levels
                ctrl.FullButton.clicked += () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        UniqueLinkedList<LevelSO> subLevels = LevelManager.GetSubLevels(_visibleLevels[index]); // Retrieve all sub-levels

                        // Open the main level first
                        string path = AssetDatabase.GUIDToAssetPath(_visibleLevels[index].SceneReference.AssetGUID);
                        EditorSceneManager.OpenScene(path);

                        // Open sub-levels additively
                        foreach (LevelSO level in subLevels)
                        {
                            path = AssetDatabase.GUIDToAssetPath(level.SceneReference.AssetGUID);
                            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                        }
                    }
                };

                // Unload button: Unload this level and it sub-levels
                ctrl.Unloadbutton.clicked += () =>
                {
                    UniqueLinkedList<LevelSO> subLevels = LevelManager.GetSubLevels(_visibleLevels[index]); // Retrieve all sub-levels
                    Scene[] scenesToClose = new Scene[subLevels.Count];

                    int i = 0;
                    foreach (LevelSO level in subLevels)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(level.SceneReference.AssetGUID);
                        scenesToClose[i] = EditorSceneManager.GetSceneByPath(path);
                        i++;
                    }

                    if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenesToClose))
                    {
                        foreach (Scene scene in scenesToClose)
                        {
                            EditorSceneManager.CloseScene(scene, true);
                        }
                    }
                };
            };
            RefreshListView();
        }
        private void RefreshListView()
        {
            _visibleLevels = (from ctx in _levelContexts
                              where ctx.Visible
                              orderby ctx.Order
                              select ctx.Level).ToList();
            _listView.itemsSource = _visibleLevels;
            _listView.RefreshItems();
        }
    }
}