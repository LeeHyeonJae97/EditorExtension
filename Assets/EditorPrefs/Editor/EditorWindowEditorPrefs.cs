using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorWindowEditorPrefs : EditorWindow
{
    [SerializeField]
    EditorPrefsBoolDictionary _bool;

    [SerializeField]
    EditorPrefsIntDictionary _int;

    [SerializeField]
    EditorPrefsFloatDictionary _float;

    [SerializeField]
    EditorPrefsStringDictionary _string;

    [MenuItem("Tools/Editor Prefs")]
    static void Init()
    {
        GetWindow<EditorWindowEditorPrefs>("Editor Prefs");
    }

    void OnEnable()
    {
        if (UnityEditor.EditorPrefs.HasKey(nameof(_bool)))
        {
            _bool = JsonUtility.FromJson<EditorPrefsBoolDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_bool)));
        }
        else
        {
            _bool = new EditorPrefsBoolDictionary();
        }

        if (UnityEditor.EditorPrefs.HasKey(nameof(_int)))
        {
            _int = JsonUtility.FromJson<EditorPrefsIntDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_int)));
        }
        else
        {
            _int = new EditorPrefsIntDictionary();
        }

        if (UnityEditor.EditorPrefs.HasKey(nameof(_float)))
        {
            _float = JsonUtility.FromJson<EditorPrefsFloatDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_float)));
        }
        else
        {
            _float = new EditorPrefsFloatDictionary();
        }

        if (UnityEditor.EditorPrefs.HasKey(nameof(_string)))
        {
            _string = JsonUtility.FromJson<EditorPrefsStringDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_string)));
        }
        else
        {
            _string = new EditorPrefsStringDictionary();
        }
    }

    void OnDisable()
    {
        _bool = null;
        _int = null;
        _float = null;
        _string = null;
    }

    void OnGUI()
    {
        var serializedObject = new SerializedObject(this);

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_bool)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_int)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_float)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_string)));

        EditorGUILayout.Space(5f);
        if (GUILayout.Button("Delete All"))
        {
            UnityEditor.EditorPrefs.DeleteAll();

            OnEnable();
        }

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            UnityEditor.EditorPrefs.SetString(nameof(_bool), JsonUtility.ToJson(_bool));
            UnityEditor.EditorPrefs.SetString(nameof(_int), JsonUtility.ToJson(_int));
            UnityEditor.EditorPrefs.SetString(nameof(_float), JsonUtility.ToJson(_float));
            UnityEditor.EditorPrefs.SetString(nameof(_string), JsonUtility.ToJson(_string));
        }
    }
}