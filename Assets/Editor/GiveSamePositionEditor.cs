using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GiveSamePositionEditor : EditorWindow {

    public GameObject SourcePositions;
    public GameObject TargetPosition;
    public GameObject NewObject;

    [MenuItem("Window/Give Positions")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GiveSamePositionEditor));
    }

    void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty sp = so.FindProperty("SourcePositions");
        SerializedProperty tp = so.FindProperty("TargetPosition");
        SerializedProperty no = so.FindProperty("NewObject");

        EditorGUILayout.PropertyField(sp, true); // True means show children
        EditorGUILayout.PropertyField(tp, true);
        EditorGUILayout.PropertyField(no, true);

        if (GUILayout.Button("Creat new objects with source positions"))
        {
            foreach(Transform t in SourcePositions.transform)
            {
                GameObject newInstance = GameObject.Instantiate(NewObject);
                newInstance.transform.position = t.position;
                newInstance.transform.parent = TargetPosition.transform;
                //newInstance.transform.localPosition = Vector3.zero;
            }
        }
        so.ApplyModifiedProperties();
    }
}
