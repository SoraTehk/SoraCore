namespace SoraCore.EditorTools
{
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;

    public class VisibleLevelListEntryController
    {
        public ObjectField LevelSOField { get; private set; }
        public Toggle AlwaysToggle { get; set; }

        public VisibleLevelListEntryController(VisualElement ve)
        {
            LevelSOField = ve.Q<ObjectField>("LevelSO-field");
            AlwaysToggle = ve.Q<Toggle>("always-toggle");
        }
    }
}