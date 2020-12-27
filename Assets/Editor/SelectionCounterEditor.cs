using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Counts selected items in hierarchy and moves them as children to target 
/// object.
/// </summary>
public class SelectionCounterEditor : EditorWindow {

    public List<Object> selectionArray;

    void OnEnable()
    {
        selectionArray = new List<Object>();
    }

    [MenuItem("Window/Selection Mover")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SelectionCounterEditor));
    }

    /// <summary>
    /// To display array taken from: http://answers.unity3d.com/questions/859554/editorwindow-display-array-dropdown.html 
    /// </summary>
    void OnGUI()
    {
        // "target" can be any class derrived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty sp = so.FindProperty("selectionArray");

        int selectionCount = Selection.objects.Length;
        EditorGUILayout.LabelField("Number of selected objects: " + selectionCount);
        if (GUILayout.Button("Save selection to array"))
        {
            selectionArray.Clear();
            foreach (Object o in Selection.objects)
            {
                selectionArray.Add(o);
            }            
        }
        EditorGUILayout.PropertyField(sp, true); // True means show children
        if(GUILayout.Button("Move to new selection"))
        {
            if(Selection.objects.Length == 1)
            {
                if(selectionArray.Count > 0)
                {
                    foreach(Object objectInArray in selectionArray)
                    {
                        GameObject targetGO = (GameObject)Selection.objects[0];
                        GameObject gameObjectInArray = (GameObject)objectInArray;
                        gameObjectInArray.transform.parent = targetGO.transform;
                    }
                }
            }
        }
        so.ApplyModifiedProperties();
    }
}
