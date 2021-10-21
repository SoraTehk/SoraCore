using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using MyBox;

using static Lean.Pool.LeanGameObjectPool;

namespace Sora.Manager {
    [CreateAssetMenu(fileName = "PrefabData", menuName = "Sora/Data/PrefabData")]
    public class PrefabDataSO : ScriptableObject {
        [ReadOnly] public string tag;
        [Space(20)]
        public GameObject prefab;
        public bool enablePooling;
        [ConditionalField("enablePooling")] public NotificationType notification;
        [ConditionalField("enablePooling")] public StrategyType strategy;
        [ConditionalField("enablePooling")] public int preload = 0;
        [ConditionalField("enablePooling")] public int capacity = 0;
        [ConditionalField("enablePooling")] public bool recycle;
        [ConditionalField("enablePooling")] public bool persist;
        [ConditionalField("enablePooling")] public bool stamp = true;
        [ConditionalField("enablePooling")] public bool warnings = true;

        void OnValidate() {
            tag = prefab ? prefab.name : "";
        }

        public void ApplyPoolConfigTo(LeanGameObjectPool instance) {
            instance.Prefab         = prefab;
            instance.Notification   = notification;
            instance.Strategy       = strategy;
            instance.Preload        = preload;
            instance.Capacity       = capacity;
            instance.Recycle        = recycle;
            instance.Persist        = persist;
            instance.Stamp          = stamp;
            instance.Warnings       = warnings;
        }
    }
}

