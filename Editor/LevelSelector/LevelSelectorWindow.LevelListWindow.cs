using SoraCore.Collections;
using SoraCore.Manager;

namespace SoraCore.EditorTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
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

            private List<LevelContext> _levelContexts
            {
                get => _selector._levelContexts;
                set => _selector._levelContexts = value;
            }

            private LevelSelectorWindow _selector;
            private ListView _listView;

            public void Init(LevelSelectorWindow selector) => _selector = selector;


            private async void CreateGUI()
            {
                // Race condition when using GetWindow<T> so we have to await
                Task timeOutTask = Task.Delay(timeOutInMili);
                while (_selector is null)
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

                TemplateContainer root = _selector.LevelListUXML.Instantiate();
                rootVisualElement.Add(root);

                // Level list view
                _listView = root.Q<ListView>("level-lv");
                SetupListView();

                // Refresh button
                root.Q<Button>("refresh-btn").clicked += () =>
                {
                    _selector.RefreshData();
                    RefreshListView();
                };
            }

            private void SetupListView()
            {
                _listView.makeItem = () =>
                {
                    TemplateContainer entry = _selector.LevelListEntryUXML.Instantiate();
                    entry.userData = new LevelListEntryController(entry);
                    return entry;
                };
                _listView.bindItem = (item, index) =>
                {
                    var ctrl = item.userData as LevelListEntryController;

                    // Object field
                    ctrl.LevelSOField.value = _levelContexts[index].Level;

                    // Visiblity in selector toggle
                    ctrl.IncludeToggle.value = _levelContexts[index].Visible;
                    ctrl.IncludeToggle.RegisterValueChangedCallback(evt =>
                    {
                        _levelContexts[index].Visible = evt.newValue;
                        _selector.RefreshListView();
                    });
                };
                
                RefreshListView();
            }

            private void RefreshListView()
            {
                _listView.itemsSource = _selector._levelContexts;
                _listView.RefreshItems();
            }
        }
    }
}