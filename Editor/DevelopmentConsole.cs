using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

public class DevelopmentConsole : EditorWindow
{
    [MenuItem("SoraCore/Development Console")]
    public static void ShowWindow()
    {
        DevelopmentConsole window = GetWindow<DevelopmentConsole>();
        window.titleContent = new GUIContent("Development Console");
    }

    private ScrollView _rightPane;

    public void CreateGUI()
    {
        TwoPaneSplitView splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);

        // Search for all scene assets
        List<string> sceneAssetPathList = new();

        string[] assetGUIDS = AssetDatabase.FindAssets("t:SceneAsset");
        foreach(string guid in assetGUIDS)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Add it to the list
            sceneAssetPathList.Add(path);
        }

        ListView leftPaneListView = new();
        splitView.Add(leftPaneListView);

        // TODO: More visual pleasing scene selector; Code looking messy, maybe a refactor?
        leftPaneListView.itemsSource = sceneAssetPathList;
        leftPaneListView.makeItem = () => {
            VisualElement ve = new();
            // Hierarchy2 extension?
            ve.style.flexDirection = FlexDirection.Row;
            ve.style.alignItems = Align.Center;
            ve.style.justifyContent = Justify.SpaceBetween;

            // ElementAt(0)
            ve.Add(new Label());

            // ElementAt(1)
            Button button = new();
            button.text = "Open";
            button.style.marginLeft = new StyleLength(StyleKeyword.Auto);
            ve.Add(button);

            // ElementAt(2)
            button = new();
            button.text = "Add";
            ve.Add(button);

            // ElementAt(3)
            button = new();
            button.text = "Remove";
            ve.Add(button);

            return ve;
        };
        leftPaneListView.bindItem = (item, index) => {
            Label label = item.ElementAt(0) as Label;
            Button openButton = item.ElementAt(1) as Button;
            Button addButton = item.ElementAt(2) as Button;
            Button removeButton = item.ElementAt(3) as Button;

            label.text = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPathList[index]).name;

            openButton.clicked += () => {
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    EditorSceneManager.OpenScene(sceneAssetPathList[index]);
                };
            addButton.clicked += () => EditorSceneManager.OpenScene((sceneAssetPathList[index]), OpenSceneMode.Additive);
            removeButton.clicked += () => {
                Scene sceneToClose = EditorSceneManager.GetSceneByPath(sceneAssetPathList[index]);

                if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { sceneToClose }))
                    EditorSceneManager.CloseScene(sceneToClose, true);
            };
        };
        leftPaneListView.onSelectionChange += OnSceneSelectionChange;

        _rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(_rightPane);
    }

    private void OnSceneSelectionChange(IEnumerable<object> items) {
        // Clear previous content from the pane
        _rightPane.Clear();

        // Get the selected scene
        SceneAsset sceneAsset = items as SceneAsset;
        if (!sceneAsset) return;

        // TODO: Make some asset reference in right panel
    }
}