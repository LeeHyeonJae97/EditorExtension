using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseInputEventOnScene : Editor
{
    void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;
                Event.current.Use();
                OnMouseDown();
                break;

            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                Event.current.Use();
                OnMouseUp();
                break;
        }
    }

    void OnMouseDown()
    {

    }

    void OnMouseUp()
    {

    }
}
