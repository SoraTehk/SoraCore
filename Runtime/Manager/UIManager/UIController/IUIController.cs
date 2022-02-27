namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.UIElements;
    using System;
    using MyBox;

    public interface IUIController {
        UIDocument Document { get; }
        void ShowUI(bool value);
    }
}
