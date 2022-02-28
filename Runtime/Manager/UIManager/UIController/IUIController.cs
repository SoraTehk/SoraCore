namespace SoraCore.Manager {
    using UnityEngine.UIElements;

    public interface IUIController {
        UIDocument Document { get; }
        void ShowUI(bool value);
    }
}
