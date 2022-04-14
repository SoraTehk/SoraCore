namespace SoraCore.EditorTools
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEngine.UIElements;

    public partial class LevelSelectorWindow
    {
        private class LevelListWindow : EditorWindow
        {
            #region Static ----------------------------------------------------------------------------------------------------
            public const int timeOutInMili = 10000;

            public static LevelListWindow ShowWindow(LevelSelectorWindow selector)
            {
                if (selector is null) throw new ArgumentNullException(nameof(selector));

                var window = GetWindow<LevelListWindow>(true, "Level List", true);
                window.Init(selector);
                return window;
            }
            #endregion

            private List<LevelContext> m_LevelContexts
            {
                get => m_Selector.m_LevelContexts;
                set => m_Selector.m_LevelContexts = value;
            }

            private LevelSelectorWindow m_Selector;
            private ListView m_ListView;

            public void Init(LevelSelectorWindow selector) => m_Selector = selector;


            private async void CreateGUI()
            {
                // Race condition when using GetWindow<T> so we have to await
                Task timeOutTask = Task.Delay(timeOutInMili);
                while (m_Selector is null)
                {
                    await Task.Yield();
                    if (timeOutTask.IsCompleted)
                    {
                        Label errorLabel = new();
                        errorLabel.text = "Timed out!";
                        rootVisualElement.Add(errorLabel);
                        return;
                    }
                }

                TemplateContainer root = m_Selector.LevelListUXML.Instantiate();
                rootVisualElement.Add(root);

                // Level list view
                m_ListView = root.Q<ListView>("level-lv");
                SetupListView();

                // Refresh button
                root.Q<Button>("refresh-btn").clicked += () =>
                {
                    m_Selector.RefreshData();
                    RefreshListView();
                };
            }

            private void SetupListView()
            {
                m_ListView.makeItem = () =>
                {
                    TemplateContainer entry = m_Selector.LevelListEntryUXML.Instantiate();
                    entry.userData = new LevelListEntryController(entry);
                    return entry;
                };
                m_ListView.bindItem = (item, index) =>
                {
                    var ctrl = item.userData as LevelListEntryController;

                    // Object field
                    ctrl.LevelSOField.value = m_LevelContexts[index].Level;

                    // Visiblity in selector toggle
                    ctrl.IncludeToggle.value = m_LevelContexts[index].IsVisible;
                    ctrl.IncludeToggle.RegisterValueChangedCallback(evt =>
                    {
                        m_LevelContexts[index].IsVisible = evt.newValue;
                        m_Selector.RefreshListView();
                    });
                };

                RefreshListView();
            }

            private void RefreshListView()
            {
                m_ListView.itemsSource = m_Selector.m_LevelContexts;
                m_ListView.RefreshItems();
            }
        }
    }
}