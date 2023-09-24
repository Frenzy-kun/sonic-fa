using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DecorationGenerator))]
public class DecorationGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DecorationGenerator myTarget = (DecorationGenerator)target;

        if (GUILayout.Button("Regenerate"))
        {
            myTarget.Regenerate();
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Clear All"))
        {
            myTarget.ClearAll();
        }
    }
}
