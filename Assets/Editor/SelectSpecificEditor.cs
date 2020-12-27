using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Selects specific objects in hierarchy
/// </summary>
public class SelectSpecificEditor : EditorWindow {

    private string objectName;

    [MenuItem("Window/Select specific")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SelectSpecificEditor));
    }

    void OnGUI()
    {
        objectName = EditorGUILayout.TextField("Name of object: ", objectName);
        if (GUILayout.Button("Select matching"))
        {
            if(Selection.objects.Length == 1)
            {
                //Parent game object whose children are to be matched
                GameObject parentGO = (GameObject)Selection.objects[0];
                int i = 0;
                //Clear all previous selections
                while(i < Selection.transforms.Length)
                {
                    Selection.transforms[i] = null;
                    i++;
                }
                i = 0;
                //Count all matches
                foreach (Transform t in parentGO.transform)
                {
                    if(t.name == objectName)
                        i++;
                }
                Object[] objectsArray = new Object[i]; //Array to be assigned to Selection.objects
                i = 0;
                foreach (Transform t in parentGO.transform)
                {
                    if(t.name == objectName)
                    {
                        objectsArray[i] = (Object)t.gameObject;
                        i++;
                    }                    
                }
                Selection.objects = objectsArray;
            }
                        
        }
        if (GUILayout.Button("Select matching in grandchildren"))
        {
            if (Selection.objects.Length == 1)
            {
                //Parent game object whose children are to be matched
                GameObject parentGO = (GameObject)Selection.objects[0];
                int i = 0;
                //Clear all previous selections
                while (i < Selection.transforms.Length)
                {
                    Selection.transforms[i] = null;
                    i++;
                }
                i = 0;
                //Count all matches
                foreach (Transform t in parentGO.GetComponentsInChildren<Transform>())
                {
                    if (t.name == objectName)
                        i++;
                }
                Object[] objectsArray = new Object[i]; //Array to be assigned to Selection.objects
                i = 0;
                foreach (Transform t in parentGO.GetComponentsInChildren<Transform>())
                {
                    if (t.name == objectName)
                    {
                        objectsArray[i] = (Object)t.gameObject;
                        i++;
                    }
                }
                Selection.objects = objectsArray;
            }            
        }
        if (GUILayout.Button("Select contains"))
        {
            if (Selection.objects.Length == 1)
            {
                //Parent game object whose children are to be matched
                GameObject parentGO = (GameObject)Selection.objects[0];
                int i = 0;
                //Clear all previous selections
                while (i < Selection.transforms.Length)
                {
                    Selection.transforms[i] = null;
                    i++;
                }
                i = 0;
                //Count all matches
                foreach (Transform t in parentGO.transform)
                {
                    if (t.name.Contains(objectName))
                        i++;
                }
                Object[] objectsArray = new Object[i]; //Array to be assigned to Selection.objects
                i = 0;
                foreach (Transform t in parentGO.transform)
                {
                    if (t.name.Contains(objectName))
                    {
                        objectsArray[i] = (Object)t.gameObject;
                        i++;
                    }
                }
                Selection.objects = objectsArray;
            }
        }
        if (GUILayout.Button("Select contains in grandchildren"))
        {
            if (Selection.objects.Length == 1)
            {
                //Parent game object whose children are to be matched
                GameObject parentGO = (GameObject)Selection.objects[0];
                int i = 0;
                //Clear all previous selections
                while (i < Selection.transforms.Length)
                {
                    Selection.transforms[i] = null;
                    i++;
                }
                i = 0;
                //Count all matches
                foreach (Transform t in parentGO.GetComponentsInChildren<Transform>())
                {
                    if (t.name.Contains(objectName))
                        i++;
                }
                Object[] objectsArray = new Object[i]; //Array to be assigned to Selection.objects
                i = 0;
                foreach (Transform t in parentGO.GetComponentsInChildren<Transform>())
                {
                    if (t.name.Contains(objectName))
                    {
                        objectsArray[i] = (Object)t.gameObject;
                        i++;
                    }
                }
                Selection.objects = objectsArray;
            }

        }
    }
}
