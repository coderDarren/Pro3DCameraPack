using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Pro3DCamera {
    [CustomEditor(typeof(CameraControl))]
    public class CameraSettingsWindow : Editor {

        CameraControl camControl;
        SerializedProperty dataManager;
        SerializedProperty _target;

        void OnEnable()
        {
            _target = serializedObject.FindProperty("target");
            dataManager = serializedObject.FindProperty("dataManager");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Settings", GUILayout.Width(Screen.width * 0.98f)))
                CameraSettingsEditorWindow.OpenWindow();

            serializedObject.Update();

            EditorGUILayout.PropertyField(dataManager);
            EditorGUILayout.PropertyField(_target);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
