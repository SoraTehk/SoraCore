using SoraCore.Collections;
using SoraCore.Manager;

namespace SoraCore.EditorTools {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public enum InteractingType
    {
        Single, // Only the selected level
        Full, // The selected level and it sub-levels
    }

    public partial class LevelSelectorWindow : EditorWindow, IHasCustomMenu {
        #region Static ----------------------------------------------------------------------------------------------------
        [MenuItem("Tools/SoraCore/Level Selector")]
        public static void ShowWindow() {
            GetWindow<LevelSelectorWindow>("Level Selector");
        }
        #endregion

        public VisualTreeAsset LevelSelectorUXML;
        public VisualTreeAsset VisibleListEntryUXML;
        public VisualTreeAsset LevelListUXML;
        public VisualTreeAsset LevelListEntryUXML;

        private List<LevelContext> _levelContexts
        {
            get => _data.LevelContexts;
            set => _data.LevelContexts = value;
        }

        private EnumField _interactBehaviourEF;
        private Toggle _additiveToggle;

        private ListView _levelLV;
        private List<LevelContext> _visibleLevelContexts;
        private List<LevelContext> _persistentLevelContexts;
        private LevelListWindow _levelListWindow;

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open Level List"), false, () => _levelListWindow = LevelListWindow.ShowWindow(this));
        }

        private void OnEnable()
        {
            EditorPrefsKey = $"SoraCore.EditorTools.LevelSelector.{Application.companyName}.{Application.productName}";
            LoadData();
        }

        private void OnDisable()
        {
            if (_levelListWindow != null) _levelListWindow.Close();
            SaveData();
        }

        private void CreateGUI() {
            TemplateContainer root = LevelSelectorUXML.Instantiate();
            rootVisualElement.Add(root);

            _interactBehaviourEF = root.Q<EnumField>("interact-behaviour-ef");
            _interactBehaviourEF.value = _data.InteractBehaviourType;
            _interactBehaviourEF.RegisterValueChangedCallback(evt => _data.InteractBehaviourType = (InteractingType)evt.newValue);

            _additiveToggle = root.Q<Toggle>("additive-toggle");
            _additiveToggle.value = _data.AdditiveToggle;
            _additiveToggle.RegisterValueChangedCallback(evt => _data.AdditiveToggle = evt.newValue);

            _levelLV = root.Q<ListView>("level-lv");
            SetupListView();
        }

        private void SetupListView()
        {
            _levelLV.makeItem = () =>
            {
                TemplateContainer entry = VisibleListEntryUXML.Instantiate();
                entry.userData = new VisibleLevelListEntryController(entry);
                return entry;
            };
            _levelLV.bindItem = (item, index) =>
            {
                var ctrl = item.userData as VisibleLevelListEntryController;

                // Object field
                ctrl.LevelSOField.value = _visibleLevelContexts[index].Level;

                ctrl.AlwaysToggle.value = _visibleLevelContexts[index].IsPersistent;
                ctrl.AlwaysToggle.RegisterValueChangedCallback(evt =>
                {
                    _visibleLevelContexts[index].IsPersistent = evt.newValue;
                    UpdatePersistentLevelList();
                });
            };
            _levelLV.onItemsChosen += OnItemsChosen;
            RefreshListView();
            UpdatePersistentLevelList();
        }

        private void OnItemsChosen(IEnumerable<object> obj)
        {
            LevelContext ctx = obj.First() as LevelContext; // Selected level context

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string path;
                UniqueLinkedList<LevelSO> levelsToOpen;

                // We have persistent level(s) to load
                if (_persistentLevelContexts.Count > 0)
                {
                    // Open the first persistent level
                    path = AssetDatabase.GUIDToAssetPath(_persistentLevelContexts[0].Level.SceneReference.AssetGUID);
                    EditorSceneManager.OpenScene(path, _additiveToggle.value ? OpenSceneMode.Additive : OpenSceneMode.Single);

                    // Retrieve all sub-levels of the first persistent level
                    levelsToOpen = LevelManager.GetSubLevels(_persistentLevelContexts[0].Level);

                    foreach (LevelContext persistentCtx in _persistentLevelContexts)
                    {
                        // Add the other persistent level
                        levelsToOpen.AddLast(persistentCtx.Level);

                        // Retrieve & add all sub-levels of other persistent level
                        LevelManager.GetSubLevels(persistentCtx.Level, ref levelsToOpen);
                    }

                    // Add the selected level
                    levelsToOpen.AddLast(ctx.Level);
                }
                else
                {
                    // Open the selected level
                    path = AssetDatabase.GUIDToAssetPath(ctx.Level.SceneReference.AssetGUID);
                    EditorSceneManager.OpenScene(path, _additiveToggle.value ? OpenSceneMode.Additive : OpenSceneMode.Single);

                    levelsToOpen = new();
                }

                // Interact Behaviour was set to 'Full'
                if (_interactBehaviourEF.value.Equals(InteractingType.Full))
                {
                    // Retrieve & add all sub-levels of selected level
                    LevelManager.GetSubLevels(ctx.Level, ref levelsToOpen);
                }
                
                // Open levels additively
                foreach (LevelSO level in levelsToOpen)
                {
                    path = AssetDatabase.GUIDToAssetPath(level.SceneReference.AssetGUID);
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
            }
        }

        private void RefreshListView()
        {
            _visibleLevelContexts = (from ctx in _levelContexts
                                     where ctx.IsVisible
                                     select ctx).ToList();
            _levelLV.itemsSource = _visibleLevelContexts;
            _levelLV.RefreshItems();
        }

        private void UpdatePersistentLevelList()
        {
            _persistentLevelContexts = (from ctx in _levelContexts
                                        where ctx.IsPersistent
                                        select ctx).ToList();
        }
    }
}