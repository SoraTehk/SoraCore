using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sora {
    [CustomEditor(typeof(BenchmarkSO), editorForChildClasses: true)]
    public class BenchmarkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BenchmarkSO e = target as BenchmarkSO;
            if (GUILayout.Button("Execute"))
                e?.Execute();
            if (GUILayout.Button("Clear"))
                e?.Clear();
        }
    }
}
