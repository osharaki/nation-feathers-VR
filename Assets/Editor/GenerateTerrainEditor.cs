using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTerrain))]
[CanEditMultipleObjects]
public class GenerateTerrainEditor : Editor {

    public override void OnInspectorGUI()
    {
        GenerateTerrain gt = (GenerateTerrain)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Terrain"))
        {
            gt.AddTrees();
        }
    }
}
