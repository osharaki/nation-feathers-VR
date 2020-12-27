using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AspectRatioEditor : EditorWindow {

    private float customHeight;

    [MenuItem("Window/Aspect ratio adjuster")]
    
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AspectRatioEditor));
    }

    void OnGUI()
    {
        customHeight = EditorGUILayout.FloatField("Height: ", customHeight);
        if (GUILayout.Button("Adjust aspect ratio"))
        {
            if (Selection.objects.Length == 1)
            {
                GameObject selectedGO = (GameObject)Selection.objects[0];
                if(selectedGO != null)
                {
                    Image ri = selectedGO.GetComponentInChildren<Image>();
                    AspectRatio(ri);
                }                
            }
        }
    }

    /// <summary>
    /// Changes image width accordingly to maintain aspect ratio 
    /// and height of original image. 
    /// </summary>
    /// <param name="raw">Image be manipulated.</param>
    void AspectRatio(Image raw)
    {
        //Note: x component of raw.rectTransform.sizeDelta is width
        raw.SetNativeSize();
        float height;
        float width;
        height = raw.rectTransform.sizeDelta.y;
        width = raw.rectTransform.sizeDelta.x;
        //Debug.Log(raw.rectTransform.sizeDelta);

        float ratio = width / height;

        //float newWidth = ratio * 100;

        raw.rectTransform.sizeDelta = new Vector2(ratio * customHeight, customHeight);
    }
}
