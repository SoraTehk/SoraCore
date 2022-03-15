namespace SoraCore.EditorTools
{
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;

    public class LevelListEntryController
    {
        public ObjectField LevelSOField { get; private set; }
        public Toggle IncludeToggle { get; private set; }
        public LevelListEntryController(VisualElement ve)
        {
            LevelSOField = ve.Q<ObjectField>("LevelSO-field");
            IncludeToggle = ve.Q<Toggle>("include-toggle");
        }
    }
}