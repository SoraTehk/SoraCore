using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace Sora.Manager
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Sora/Manager/InputManager")]
    public class InputManager : SingletonScriptableObject<InputManager> {
        //public InputActionAsset InputActionAsset;
        //private InputAction _pauseIA;
        //public GameEvent escapeKeyDown;

        //public override void Initialize()
        //{
        //    InputActionMap CurrentInputActionMap = InputActionAsset.actionMaps[0];
        //    _pauseIA = CurrentInputActionMap.FindAction("Pause/Option");

        //    _pauseIA.started += (InputAction.CallbackContext value) => escapeKeyDown?.Invoke();
        //}

        //public override void Terminate()
        //{
        //    _pauseIA.started -= (InputAction.CallbackContext value) => escapeKeyDown?.Invoke();
        //}
    }
}
