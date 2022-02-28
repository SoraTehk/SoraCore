using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SceneInfoController {
    public Label assetPath { get; private set; }
    public ObjectField sceneAsset { get; private set; }
    public SceneInfoController(VisualElement ve) {
        assetPath = ve.Q<Label>("asset-path");
        sceneAsset = ve.Q<ObjectField>("scene-asset");
    }
}
