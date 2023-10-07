using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynamicLevelLoader))]
public class DynamicSceneLoader_Editor : Editor
{
    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if ( Event.current.button == 1)
                {
                    GUIUtility.hotControl = controlID;
                    Debug.Log("MouseDown");
                    Event.current.Use();

                    DynamicLevelLoader dll = (DynamicLevelLoader)target;
                    dll.ProcessClick(Event.current.mousePosition);
                }
                break;

            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                Event.current.Use();
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Select This object and Right Click to load a scene");
    }

    

    /*static DynamicSceneLoader_Editor()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneview)
    {
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Item 1"), false, Callback, 1);
                menu.AddItem(new GUIContent("Item 2"), false, Callback, 2);
                menu.ShowAsContext();
            }
        }
    }

    static void Callback(object obj)
    {
        // Do something
    }*/
}
