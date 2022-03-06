namespace SoraCore.Manager {
    using UnityEditor;

    //[CustomEditor(typeof(SaveableController))]
    //public class SaveableControllerEditor : Editor {

    //    public override void OnInspectorGUI() {
    //        SerializedProperty isRuntimeInstantiateProp = serializedObject.FindProperty("<IsRuntimeInstantiate>k__BackingField");
    //        SerializedProperty prefabPathInResourcesProp = serializedObject.FindProperty("<PrefabPathInResources>k__BackingField");
    //        SerializedProperty GUIDProp = serializedObject.FindProperty("<GUID>k__BackingField");

    //        EditorGUILayout.PropertyField(isRuntimeInstantiateProp);

    //        if (isRuntimeInstantiateProp.boolValue) {
    //            if (prefabPathInResourcesProp.stringValue == "") {
    //                EditorGUILayout.HelpBox("<b>Prefab Path In Resources</b> need to be assigned!", MessageType.Error);
    //            }
    //        }
    //        else {
    //            if (GUIDProp.stringValue == "") {
    //                EditorGUILayout.HelpBox("<b>GUID</b> need to be assigned!", MessageType.Error);
    //            }
    //        }

    //        using (new EditorGUI.DisabledScope(!isRuntimeInstantiateProp.boolValue)) {
    //            EditorGUILayout.PropertyField(prefabPathInResourcesProp);
    //        }
    //        using (new EditorGUI.DisabledScope(isRuntimeInstantiateProp.boolValue)) {
    //            EditorGUILayout.PropertyField(GUIDProp);
    //        }

    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}
}