using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ProjectExtension : EditorWindow
{
    static Rect selectedAsset;

    [SerializeField]
    Project _project;

    [SerializeReference]
    Group _group;

    SerializedObject _serializedObject;
    EditorGUISplitView _splitView;

    [MenuItem("Window/General/Project Extension")]
    static void Init()
    {
        GetWindow<ProjectExtension>("Project Extension").Show();
    }

    void OnEnable()
    {
        _project = new Project() { groups = new List<Group>() { new Group() { name = "ASDF", folders = new List<Folder>() { new Folder("AA", "AA"), new Folder("BB", "BB") } } } };

        _serializedObject = new SerializedObject(this);
        _splitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
    }

    void OnGUI()
    {
        _splitView.BeginSplitView();
        DrawProject();
        _splitView.Split();
        DrawSelectedGroup();
        _splitView.EndSplitView();

        GUI.Box(selectedAsset, GUIContent.none, EditorStyles.selectionRect);

        _serializedObject.ApplyModifiedProperties();
        _serializedObject.Update();
    }

    void DrawProject()
    {
        EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_project)));
    }

    void DrawSelectedGroup()
    {
        var selectedGroupProp = _serializedObject.FindProperty(nameof(_group));

        if (selectedGroupProp.managedReferenceValue != null)
        {
            EditorGUILayout.PropertyField(selectedGroupProp);
        }
    }

    [Serializable]
    class Project
    {
        [SerializeReference]
        public List<Group> groups;
    }

    [Serializable]
    class Group
    {
        public const string Icon = "Project@2x";

        [SerializeReference]
        public List<Folder> folders;

        public string name;

        // TODO :
        // 어디서 설정?
        //
        public bool expand;
    }

    [Serializable]
    class Asset
    {
        public string path;
        public string name;
        public string icon;
        public string guid;
        public bool edit;

        public Asset(string path, string name, string icon, string guid)
        {
            this.path = path;
            this.name = name;
            this.icon = icon;
            this.guid = guid;
        }
    }

    [Serializable]
    class Folder : Asset
    {
        public const string Icon = "Folder Icon";
        public const string IconOpen = "FolderOpened Icon";

        [SerializeReference]
        public List<Asset> assets;

        public Folder(string path, string name) : base(path, name, "", "")
        {
            this.assets = new List<Asset>();
        }
    }

    [CustomPropertyDrawer(typeof(Project))]
    class ProjectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var groupProp = property.FindPropertyRelative("groups");

            for (int i = 0; i < groupProp.arraySize; i++)
            {
                var prop = groupProp.GetArrayElementAtIndex(i);

                if (i == 0)
                {
                    EditorGUI.PropertyField(position, prop, true);
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(Group))]
    class GroupPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, property);
            HandleEvent(position, property);
        }

        void Draw(Rect position, SerializedProperty property)
        {
            if (property.FindPropertyRelative("expand").boolValue)
            {
                var folderProp = property.FindPropertyRelative("folders");

                for (int i = 0; i < folderProp.arraySize; i++)
                {
                    if (i == 0)
                    {
                        EditorGUI.PropertyField(position, folderProp.GetArrayElementAtIndex(i), true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(folderProp.GetArrayElementAtIndex(i), true);
                    }
                }
            }
            else
            {
                var content = new GUIContent($" {property.FindPropertyRelative("name").stringValue}", EditorGUIUtility.IconContent(Group.Icon).image);

                EditorGUI.LabelField(position, content);
            }
        }

        void HandleEvent(Rect position, SerializedProperty property)
        {
            Event evt = Event.current;

            var type = evt.type;
            var button = evt.button;
            var clickCount = evt.clickCount;
            var contains = position.Contains(evt.mousePosition);

            HandleClick();

            void HandleClick()
            {
                if (type == EventType.MouseDown && clickCount == 1 && contains)
                {
                    if (button == 0)
                    {
                        property.serializedObject.FindProperty("_group").managedReferenceValue = property.managedReferenceValue as Group;

                        if (property.serializedObject.targetObject is ProjectExtension proejctExtension)
                        {
                            proejctExtension.Repaint();
                        }
                    }
                    else if (button == 1)
                    {
                        var context = new GenericMenu();

                        context.AddItem(new GUIContent(!(property.managedReferenceValue as Asset).edit ? "Edit" : "End Edit"), false, Edit);
                        context.AddItem(new GUIContent("Add Group"), false, AddGroup);
                        context.AddItem(new GUIContent("Remove Group"), false, RemoveGroup);
                        context.ShowAsContext();
                    }
                }
            }

            void AddGroup()
            {
                // TODO :
                //
            }

            void RemoveGroup()
            {
                // TODO :
                //
            }

            void Edit()
            {
                (property.managedReferenceValue as Asset).edit = !(property.managedReferenceValue as Asset).edit;
            }
        }
    }

    [CustomPropertyDrawer(typeof(Asset))]
    class AssetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HandleEvent(position, property);
            Draw(position, property);
        }

        void Draw(Rect position, SerializedProperty property)
        {
            if ((property.managedReferenceValue as Asset).edit)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), GUIContent.none);
            }
            else
            {
                var content = new GUIContent($" {property.FindPropertyRelative("name").stringValue}", EditorGUIUtility.IconContent("SceneAsset Icon").image);

                EditorGUI.LabelField(position, content);
            }
        }

        void HandleEvent(Rect position, SerializedProperty property)
        {
            Event evt = Event.current;

            var type = evt.type;
            var button = evt.button;
            var clickCount = evt.clickCount;
            var contains = position.Contains(evt.mousePosition);

            HandleClick();
            HandleDragAndDrop();

            void HandleClick()
            {
                if (type == EventType.MouseDown && contains)
                {
                    if (button == 0)
                    {
                        if (clickCount == 1)
                        {
                            selectedAsset = position;

                            if (property.serializedObject.targetObject is ProjectExtension proejctExtension)
                            {
                                proejctExtension.Repaint();
                            }

                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(property.FindPropertyRelative("assetPath").stringValue);
                        }
                        else if (clickCount == 2)
                        {
                            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(property.FindPropertyRelative("assetPath").stringValue));
                        }
                    }
                    else if (button == 1)
                    {
                        if (clickCount == 1)
                        {
                            var context = new GenericMenu();

                            context.AddItem(new GUIContent(!(property.managedReferenceValue as Asset).edit ? "Edit" : "End Edit"), false, Edit);
                            context.ShowAsContext();
                        }
                    }
                }
            }

            void HandleDragAndDrop()
            {
                switch (type)
                {
                    case EventType.DragUpdated:
                        {
                            if (contains && DragAndDrop.paths.Length > 0)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            }
                            break;
                        }
                    case EventType.DragPerform:
                        {
                            if (contains && DragAndDrop.paths.Length > 0)
                            {
                                // TODO :
                                // 동일한 guid를 갖는 에셋이 없는 경우 다른 에셋을 드래그 앤 드랍해 갱신
                                //
                            }
                            break;
                        }
                }
            }

            void Edit()
            {
                (property.managedReferenceValue as Asset).edit = !(property.managedReferenceValue as Asset).edit;
            }
        }
    }

    [CustomPropertyDrawer(typeof(Folder))]
    class FolderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HandleEvent(position, property);
            Draw(position, property);
        }

        void Draw(Rect position, SerializedProperty property)
        {
            if ((property.managedReferenceValue as Asset).edit)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), GUIContent.none);
            }
            else
            {
                var content = new GUIContent($" {property.FindPropertyRelative("name").stringValue}", EditorGUIUtility.IconContent(property.isExpanded ? Folder.IconOpen : Folder.Icon).image);

                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, content);
            }

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var assetsProp = property.FindPropertyRelative("assets");

                for (int i = 0; i < assetsProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(assetsProp.GetArrayElementAtIndex(i), true);
                }

                EditorGUI.indentLevel--;
            }
        }

        void HandleEvent(Rect position, SerializedProperty property)
        {
            Event evt = Event.current;

            var type = evt.type;
            var button = evt.button;
            var clickCount = evt.clickCount;
            var contains = position.Contains(evt.mousePosition);

            HandleClick();
            HandleDragAndDrop();

            void HandleClick()
            {
                if (type == EventType.MouseDown && clickCount == 1 && contains)
                {
                    if (button == 0)
                    {
                        selectedAsset = position;

                        if (property.serializedObject.targetObject is ProjectExtension proejctExtension)
                        {
                            proejctExtension.Repaint();
                        }
                    }
                    else if (button == 1)
                    {
                        var context = new GenericMenu();

                        context.AddItem(new GUIContent(!(property.managedReferenceValue as Asset).edit ? "Edit" : "End Edit"), false, Edit);
                        context.AddItem(new GUIContent("Add Folder"), false, AddFolder);
                        context.AddItem(new GUIContent("Remove Folder"), false, RemoveFolder);
                        context.ShowAsContext();
                    }
                }
            }

            void HandleDragAndDrop()
            {
                switch (type)
                {
                    case EventType.DragUpdated:
                        {
                            if (contains && DragAndDrop.paths.Length > 0)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            }
                            break;
                        }
                    case EventType.DragPerform:
                        {
                            if (contains && DragAndDrop.paths.Length > 0)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                DragAndDrop.AcceptDrag();

                                var folder = property.managedReferenceValue as Folder;

                                foreach (var obj in DragAndDrop.objectReferences)
                                {
                                    var path = AssetDatabase.GetAssetPath(obj);
                                    var guid = AssetDatabase.AssetPathToGUID(path);

                                    var icon = "";
                                    switch (obj)
                                    {
                                        case SceneAsset:
                                            icon = "SceneAsset Icon";
                                            break;
                                        case MonoScript:
                                            icon = "cs Script Icon";
                                            break;
                                    }

                                    folder.assets.Add(new Asset($"{folder.path}/{path}", path, icon, guid));
                                }
                            }
                            break;
                        }
                }
            }

            void AddFolder()
            {
                var folder = property.managedReferenceValue as Folder;

                folder.assets.Add(new Folder($"{folder.path}/New Folder", "New Folder"));
            }

            void RemoveFolder()
            {
                // TODO :
                //
            }

            void Edit()
            {
                (property.managedReferenceValue as Asset).edit = !(property.managedReferenceValue as Asset).edit;
            }
        }
    }
}
#endif
