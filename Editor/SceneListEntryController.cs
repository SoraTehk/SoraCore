using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SceneListEntryController
{
    public Label sceneName { get; private set; }
    public Button openButton { get; private set; }
    public Button addButton { get; private set; }
    public Button removeButton { get; private set; }
    public SceneListEntryController(VisualElement ve) {
        sceneName = ve.Q<Label>("scene-name");
        openButton = ve.Q<Button>("open-button");
        addButton = ve.Q<Button>("add-button");
        removeButton = ve.Q<Button>("remove-button");
    }
}
