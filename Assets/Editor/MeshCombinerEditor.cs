using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector script to combine object meshes in edit mode.
/// </summary>
[CustomEditor(typeof(MeshCombiner))]
[CanEditMultipleObjects]
public class MeshCombinerEditor : Editor {

    SerializedProperty lookAtPoint;

    void OnEnable()
    {
        lookAtPoint = serializedObject.FindProperty("lookAtPoint");
    }

    public override void OnInspectorGUI()
    {
        MeshCombiner mc = (MeshCombiner)target;
        DrawDefaultInspector();
        if(GUILayout.Button("Combine Meshes"))
        {
            mc.CombineMeshes();
        }
        if(GUILayout.Button("Advanced Combine"))
        {
            mc.AdvancedCombine();
        }
        //serializedObject.Update();
        //EditorGUILayout.PropertyField(lookAtPoint);
        //serializedObject.ApplyModifiedProperties();
    }
}
