namespace SoraCore.EditorTools
{
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;

    public class VisibleLevelListEntryController
    {
        public ObjectField LevelSOField { get; private set; }
        public Button SingleButton { get; private set; }
        public Button FullButton { get; private set; }
        public Button Unloadbutton { get; private set; }
        public VisibleLevelListEntryController(VisualElement ve)
        {
            LevelSOField = ve.Q<ObjectField>("LevelSO-field");
            SingleButton = ve.Q<Button>("single-button");
            FullButton = ve.Q<Button>("full-button");
            Unloadbutton = ve.Q<Button>("unload-button");
        }
    }
}