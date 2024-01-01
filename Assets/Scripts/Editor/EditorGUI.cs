using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
public static class EditorGUI
{
    public static void DrawSpace(float width)
    {
        EditorGUILayout.Space(width);
    }

    public static void DrawProperty(SerializedProperty property, Style style = null)
    {
        if (style == null)
        {
            EditorGUILayout.PropertyField(property);
        }
        else
        {
            EditorGUILayout.PropertyField(property, ToGUILayoutOptions(style));
        }
    }

    public static void DrawLabel(string label, Style style = null)
    {
        if (style == null)
        {
            EditorGUILayout.LabelField(label);
        }
        else
        {
            EditorGUILayout.LabelField(label, ToGUILayoutOptions(style));
        }
    }

    public static void DrawInputField(string label, Style labelStyle = null, Style inputFieldStyle = null)
    {
        EditorGUILayout.BeginHorizontal();

        if (labelStyle == null)
        {
            EditorGUILayout.LabelField(label);
        }
        else
        {
            EditorGUILayout.LabelField(label, ToGUILayoutOptions(labelStyle));
        }

        if (inputFieldStyle == null)
        {
            Input.Set(label, EditorGUILayout.TextField(Input.Get(label)));
        }
        else
        {
            Input.Set(label, EditorGUILayout.TextField(Input.Get(label), ToGUILayoutOptions(inputFieldStyle)));
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void DrawInputArea(string label, Style labelStyle = null, Style inputAreaStyle = null)
    {
        EditorGUILayout.BeginHorizontal();

        if (labelStyle == null)
        {
            EditorGUILayout.LabelField(label);
        }
        else
        {
            EditorGUILayout.LabelField(label, ToGUILayoutOptions(labelStyle));
        }

        if (inputAreaStyle == null)
        {
            Input.Set(label, EditorGUILayout.TextArea(Input.Get(label)));
        }
        else
        {
            Input.Set(label, EditorGUILayout.TextArea(Input.Get(label), ToGUILayoutOptions(inputAreaStyle)));
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void DrawButton(string label, Action OnClick, Style style = null)
    {
        if (style == null ? GUILayout.Button(label) : GUILayout.Button(label, ToGUILayoutOptions(style)))
        {
            OnClick?.Invoke();
        }
    }

    public static void DrawToggle(string label, Action<bool> OnToggled, Style style = null)
    {
        if (style == null)
        {
            Toggle.Set(label, GUILayout.Toggle(Toggle.Get(label), label));
        }
        else
        {
            Toggle.Set(label, GUILayout.Toggle(Toggle.Get(label), label, ToGUILayoutOptions(style)));
        }

        OnToggled?.Invoke(Toggle.Get(label));
    }

    public static void GroupEnable(bool enable, Action OnDraw)
    {
        GUI.enabled = enable;

        OnDraw?.Invoke();

        GUI.enabled = true;
    }

    public static void GroupHorizontal(Action OnDraw, Style style = null)
    {
        if (style == null)
        {
            EditorGUILayout.BeginHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal(ToGUILayoutOptions(style));
        }

        OnDraw?.Invoke();

        EditorGUILayout.EndHorizontal();
    }

    public static void GroupVertical(Action OnDraw, Style style = null)
    {
        if (style != null)
        {
            EditorGUILayout.BeginVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical(ToGUILayoutOptions(style));
        }

        OnDraw?.Invoke();

        EditorGUILayout.EndVertical();
    }

    public static void GroupFoldout(string key, string header, Action OnDraw)
    {
        Foldout.Set(key, EditorGUILayout.BeginFoldoutHeaderGroup(Foldout.Get(key), header));

        if (Foldout.Get(key))
        {
            OnDraw?.Invoke();
        }
    }

    public static void GroupScroll(string key, Action OnDraw, Style style = null)
    {
        if (style == null)
        {
            EditorGUILayout.BeginScrollView(ScrollView.Get(key));
        }
        else
        {
            EditorGUILayout.BeginScrollView(ScrollView.Get(key), ToGUILayoutOptions(style));
        }

        OnDraw?.Invoke();

        EditorGUILayout.EndScrollView();
    }

    public static SerializedProperty[] GetArray(this SerializedProperty property)
    {
        if (!property.isArray) return null;

        var elements = new SerializedProperty[property.arraySize];

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = property.GetArrayElementAtIndex(i);
        }

        return elements;
    }

    public static void Foreach(this SerializedProperty property, Action<SerializedProperty> OnEnumerated)
    {
        if (!property.isArray) return;

        for (int i = 0; i < property.arraySize; i++)
        {
            OnEnumerated?.Invoke(property.GetArrayElementAtIndex(i));
        }
    }

    static GUILayoutOption[] ToGUILayoutOptions(Style style)
    {
        List<GUILayoutOption> options = new List<GUILayoutOption>();

        if (style.Width > 0)
        {
            options.Add(GUILayout.Width(style.Width));
        }
        if (style.MinWidth > 0)
        {
            options.Add(GUILayout.MinWidth(style.MinWidth));
        }
        if (style.MaxWidth > 0)
        {
            options.Add(GUILayout.MaxWidth(style.MaxWidth));
        }
        if (style.Height > 0)
        {
            options.Add(GUILayout.Height(style.Height));
        }
        if (style.MinHeight > 0)
        {
            options.Add(GUILayout.MinHeight(style.MinHeight));
        }
        if (style.MaxHeight > 0)
        {
            options.Add(GUILayout.MaxHeight(style.MaxHeight));
        }

        return options.ToArray();
    }

    class Toggle
    {
        static Dictionary<string, bool> _toggled;

        public static bool Get(string key)
        {
            if (_toggled == null)
            {
                _toggled = new Dictionary<string, bool>();
            }

            return _toggled.TryGetValue(key, out var value) ? value : false;
        }

        public static void Set(string key, bool value)
        {
            if (_toggled == null)
            {
                _toggled = new Dictionary<string, bool>();
            }

            _toggled[key] = value;
        }
    }

    class ScrollView
    {
        static Dictionary<string, Vector2> _pos;

        public static Vector2 Get(string key)
        {
            if (_pos == null)
            {
                _pos = new Dictionary<string, Vector2>();
            }

            return _pos.TryGetValue(key, out var value) ? value : Vector2.zero;
        }

        public static void Set(string key, Vector2 value)
        {
            if (_pos == null)
            {
                _pos = new Dictionary<string, Vector2>();
            }

            _pos[key] = value;
        }
    }

    class Foldout
    {
        static Dictionary<string, bool> _foldout;

        public static bool Get(string key)
        {
            if (_foldout == null)
            {
                _foldout = new Dictionary<string, bool>();
            }

            return _foldout.TryGetValue(key, out var value) ? value : false;
        }

        public static void Set(string key, bool value)
        {
            if (_foldout == null)
            {
                _foldout = new Dictionary<string, bool>();
            }

            _foldout[key] = value;
        }
    }

    class Input
    {
        static Dictionary<string, string> _input;

        public static string Get(string key)
        {
            if (_input == null)
            {
                _input = new Dictionary<string, string>();
            }

            return _input.TryGetValue(key, out var value) ? value : "";
        }

        public static void Set(string key, string value)
        {
            if (_input == null)
            {
                _input = new Dictionary<string, string>();
            }

            _input[key] = value;
        }
    }

    public class Style
    {
        public int Width { get; set; }

        public int MinWidth { get; set; }

        public int MaxWidth { get; set; }

        public int Height { get; set; }

        public int MinHeight { get; set; }

        public int MaxHeight { get; set; }
    }
}
#endif
