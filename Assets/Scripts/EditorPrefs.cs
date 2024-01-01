using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorPrefs
{
    static EditorPrefsBoolDictionary _bool;
    static EditorPrefsIntDictionary _int;
    static EditorPrefsFloatDictionary _float;
    static EditorPrefsStringDictionary _string;

    static EditorPrefs()
    {
        if (UnityEditor.EditorPrefs.HasKey(nameof(_bool)))
        {
            _bool = JsonUtility.FromJson<EditorPrefsBoolDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_bool)));
        }
        if (UnityEditor.EditorPrefs.HasKey(nameof(_int)))
        {
            _int = JsonUtility.FromJson<EditorPrefsIntDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_int)));
        }
        if (UnityEditor.EditorPrefs.HasKey(nameof(_float)))
        {
            _float = JsonUtility.FromJson<EditorPrefsFloatDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_float)));
        }
        if (UnityEditor.EditorPrefs.HasKey(nameof(_string)))
        {
            _string = JsonUtility.FromJson<EditorPrefsStringDictionary>(UnityEditor.EditorPrefs.GetString(nameof(_string)));
        }
    }

    public static bool HasKey(string key)
    {
        return _bool.ContainsKey(key) || _int.ContainsKey(key) || _float.ContainsKey(key) || _string.ContainsKey(key);
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        return _bool != null && _bool.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static int GetInt(string key, int defaultValue)
    {
        return _int != null && _int.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static float GetFloat(string key, float defaultValue)
    {
        return _float != null && _float.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static string GetString(string key, string defaultValue)
    {
        return _string != null && _string.TryGetValue(key, out var value) ? value : defaultValue;
    }
}

[System.Serializable]
public class EditorPrefsBoolDictionary : SerializableDictionary<string, bool>
{

}

[System.Serializable]
public class EditorPrefsIntDictionary : SerializableDictionary<string, int>
{

}

[System.Serializable]
public class EditorPrefsFloatDictionary : SerializableDictionary<string, float>
{

}

[System.Serializable]
public class EditorPrefsStringDictionary : SerializableDictionary<string, string>
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EditorPrefsBoolDictionary))]
public class EditorPrefsBoolDictionaryDrawer : SerializableDictionaryDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsBoolDictionary.KeyValuePair))]
public class EditorPrefsBoolKeyValuePairDrawer : SerializableDictionaryKeyValuePairDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsIntDictionary))]
public class EditorPrefsIntDictionaryDrawer : SerializableDictionaryDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsIntDictionary.KeyValuePair))]
public class EditorPrefsIntKeyValuePairDrawer : SerializableDictionaryKeyValuePairDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsFloatDictionary))]
public class EditorPrefsFloatDictionaryDrawer : SerializableDictionaryDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsFloatDictionary.KeyValuePair))]
public class EditorPrefsFloatKeyValuePairDrawer : SerializableDictionaryKeyValuePairDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsStringDictionary))]
public class EditorPrefsStringDictionaryDrawer : SerializableDictionaryDrawer
{

}

[CustomPropertyDrawer(typeof(EditorPrefsStringDictionary.KeyValuePair))]
public class EditorPrefsStringKeyValuePairDrawer : SerializableDictionaryKeyValuePairDrawer
{

}
#endif