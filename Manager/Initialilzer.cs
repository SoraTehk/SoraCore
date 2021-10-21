using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Manager {
    public class Initialilzer : MonoBehaviour
    {
        public AudioManager AudioManager;
        public GameManager GameManager;
        public UIManager UIManager;
        public PrefabManager PrefabManager;
        public InputManager InputManager;

        void Awake() {
            GameManager.LoadPersistentScene();
        }

        void Start() {
            //Not working on Awake?
            AudioManager.LoadPlayerPrefsAll();
        }
    }
}

