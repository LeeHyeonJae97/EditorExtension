using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log(EditorPrefs.GetBool("A", false));
        Debug.Log(EditorPrefs.GetBool("B", false));
        Debug.Log(EditorPrefs.GetInt("C", 0));
        Debug.Log(EditorPrefs.GetFloat("D", 0f));
        Debug.Log(EditorPrefs.GetString("E", ""));
    }
}
