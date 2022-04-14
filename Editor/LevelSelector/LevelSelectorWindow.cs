namespace SoraCore.EditorTools
{
    using Collections;
    using Manager.Level;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public enum InteractingType
    {
        Single, // Only the selected level
        Full, // The selected level and it sub-levels
    }

    public partial class LevelSelectorWindow : EditorWindow, IHasCustomMenu
    {
        #region Static ----------------------------------------------------------------------------------------------------
        [MenuItem("Tools/SoraCore/Level Selector")]
        public static void ShowWindow()
        {
            GetWindow<LevelSelectorWindow>("Level Selector");
        }
        #endregion

        public VisualTreeAsset LevelSelectorUXML;
        public VisualTreeAsset VisibleListEntryUXML;
        public VisualTreeAsset LevelListUXML;
        public VisualTreeAsset LevelListEntryUXML;

        private List<LevelContext> m_LevelContexts
        {
            get => m_Data.LevelContexts;
            set => m_Data.LevelContexts = value;
        }

        private EnumField m_InteractBehaviourEF;
        private Toggle m_AdditiveToggle;

        private ListView m_LevelLV;
        private List<LevelContext> m_VisibleLevelContexts;
        private List<LevelContext> m_PersistentLevelContexts;
        private LevelListWindow m_LevelListWindow;

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open Level List"), false, () => m_LevelListWindow = LevelListWindow.ShowWindow(this));
        }

        private void OnEnable()
        {
            EditorPrefsKey = $"SoraCore.EditorTools.LevelSelector.{Application.companyName}.{Application.productName}";
            LoadData();
        }

        private void OnDisable()
        {
            if (m_LevelListWindow != null) m_LevelListWindow.Close();
            SaveData();
        }

        private void CreateGUI()
        {
            TemplateContainer root = LevelSelectorUXML.Instantiate();
            rootVisualElement.Add(root);

            m_InteractBehaviourEF = root.Q<EnumField>("interact-behaviour-ef");
            m_InteractBehaviourEF.value = m_Data.InteractBehaviourType;
            m_InteractBehaviourEF.RegisterValueChangedCallback(evt => m_Data.InteractBehaviourType = (InteractingType)evt.newValue);

            m_AdditiveToggle = root.Q<Toggle>("additive-toggle");
            m_AdditiveToggle.value = m_Data.AdditiveToggle;
            m_AdditiveToggle.RegisterValueChangedCallback(evt => m_Data.AdditiveToggle = evt.newValue);

            m_LevelLV = root.Q<ListView>("level-lv");
            SetupListView();
        }

        private void SetupListView()
        {
            m_LevelLV.makeItem = () =>
            {
                TemplateContainer entry = VisibleListEntryUXML.Instantiate();
                entry.userData = new VisibleLevelListEntryController(entry);
                return entry;
            };
            m_LevelLV.bindItem = (item, index) =>
            {
                var ctrl = item.userData as VisibleLevelListEntryController;

                // Object field
                ctrl.LevelSOField.value = m_VisibleLevelContexts[index].Level;

                ctrl.AlwaysToggle.value = m_VisibleLevelContexts[index].IsPersistent;
                ctrl.AlwaysToggle.RegisterValueChangedCallback(evt =>
                {
                    m_VisibleLevelContexts[index].IsPersistent = evt.newValue;
                    UpdatePersistentLevelList();
                });
            };
            m_LevelLV.onItemsChosen += OnItemsChosen;
            RefreshListView();
            UpdatePersistentLevelList();
        }

        private void OnItemsChosen(IEnumerable<object> obj)
        {
            LevelContext ctx = obj.First() as LevelContext; // Selected level context

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string path;
                UniqueLinkedList<LevelAsset> levelsToOpen;

                // We have persistent level(s) to load
                if (m_PersistentLevelContexts.Count > 0)
                {
                    // Open the first persistent level
                    path = AssetDatabase.GUIDToAssetPath(m_PersistentLevelContexts[0].Level.SceneReference.AssetGUID);
                    EditorSceneManager.OpenScene(path, m_AdditiveToggle.value ? OpenSceneMode.Additive : OpenSceneMode.Single);

                    // Retrieve all sub-levels of the first persistent level
                    levelsToOpen = LevelManager.GetSubLevels(m_PersistentLevelContexts[0].Level);

                    foreach (LevelContext persistentCtx in m_PersistentLevelContexts)
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
                    EditorSceneManager.OpenScene(path, m_AdditiveToggle.value ? OpenSceneMode.Additive : OpenSceneMode.Single);

                    levelsToOpen = new();
                }

                // Interact Behaviour was set to 'Full'
                if (m_InteractBehaviourEF.value.Equals(InteractingType.Full))
                {
                    // Retrieve & add all sub-levels of selected level
                    LevelManager.GetSubLevels(ctx.Level, ref levelsToOpen);
                }

                // Open levels additively
                foreach (LevelAsset level in levelsToOpen)
                {
                    path = AssetDatabase.GUIDToAssetPath(level.SceneReference.AssetGUID);
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
            }
        }

        private void RefreshListView()
        {
            m_VisibleLevelContexts = (from ctx in m_LevelContexts
                                      where ctx.IsVisible
                                      select ctx).ToList();
            m_LevelLV.itemsSource = m_VisibleLevelContexts;
            m_LevelLV.RefreshItems();
        }

        private void UpdatePersistentLevelList()
        {
            m_PersistentLevelContexts = (from ctx in m_VisibleLevelContexts
                                         where ctx.IsPersistent
                                         select ctx).ToList();
        }
    }
}