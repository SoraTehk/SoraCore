namespace SoraCore.EditorTools {
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;
    public class SceneLoader : EditorWindow, IHasCustomMenu {
        public VisualTreeAsset sceneListEntryXML;
        public VisualTreeAsset sceneInfoXML;

        private TwoPaneSplitView _splitView;
        private TemplateContainer _rightPane;

        //[MenuItem("Tools/SoraCore/" + nameof(SceneLoader))]
        public static void ShowWindow() {
            SceneLoader window = GetWindow<SceneLoader>();
            window.titleContent = new GUIContent(nameof(SceneLoader));
        }

        public void AddItemsToMenu(GenericMenu menu) {
            menu.AddItem(new GUIContent("Change Orientation"), false, () =>
            {
                // TODO: Splitview drag-able separator cut halfway
                if (_splitView.orientation == TwoPaneSplitViewOrientation.Horizontal) {
                    _splitView.orientation = TwoPaneSplitViewOrientation.Vertical;
                }
                else {
                    _splitView.orientation = TwoPaneSplitViewOrientation.Horizontal;
                }

            });

            //menu.AddItem();
        }

        public void CreateGUI() {
            _splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(_splitView);

            List<string> sceneAssetPathList = new();
            // Search for all scenes in project and..
            string[] assetGUIDS = AssetDatabase.FindAssets("t:SceneAsset", new string[] { "Assets" });
            foreach (string guid in assetGUIDS) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // ...add it to the list
                sceneAssetPathList.Add(path);
            }

            ListView leftPane = new();
            _splitView.Add(leftPane);

            #region leftPane
            leftPane.itemsSource = sceneAssetPathList;
            leftPane.makeItem = () =>
            {
                var entry = sceneListEntryXML.Instantiate();
                entry.userData = new SceneListEntryController(entry);
                return entry;
            };
            leftPane.bindItem = (item, index) =>
            {
                var entryController = item.userData as SceneListEntryController;

                entryController.sceneName.text = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPathList[index]).name;

                entryController.openButton.clicked += () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(sceneAssetPathList[index]);
                };
                entryController.addButton.clicked += () => EditorSceneManager.OpenScene((sceneAssetPathList[index]), OpenSceneMode.Additive);
                entryController.removeButton.clicked += () =>
                {
                    Scene sceneToClose = EditorSceneManager.GetSceneByPath(sceneAssetPathList[index]);

                    if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { sceneToClose }))
                        EditorSceneManager.CloseScene(sceneToClose, true);
                };
            };
            leftPane.onSelectionChange += OnSceneSelectionChange;
            #endregion

            _rightPane = sceneInfoXML.Instantiate();
            _rightPane.userData = new SceneInfoController(_rightPane);
            _splitView.Add(_rightPane);
        }

        private void OnSceneSelectionChange(IEnumerable<object> items) {
            // Null checking (show/hide _rightPane)
            if (items.Any()) {
                _rightPane.visible = true;

            }
            else {
                _rightPane.visible = false;
                return;
            }

            // Get the selected item (only 1)
            string sceneAssetPath = items.Single().ToString();

            var infoController = _rightPane.userData as SceneInfoController;
            infoController.assetPath.text = "Path: " + sceneAssetPath;
            // TODO: Make this field blur out/read only in editor window
            infoController.sceneAsset.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
        }
    }
}