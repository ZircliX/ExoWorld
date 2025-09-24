using LTX.Tools.Editor.Annotations.Data;
using UnityEditor;
using UnityEngine;

namespace LTX.Tools.Editor.Annotations
{
    [InitializeOnLoad]
    public static class LTXAnnotationEditorDrawer
    {
        static LTXAnnotationEditorDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }


        private static bool IsEditing;
        private static Vector2 scroll;
        private static string currentText;

        private static void OnPostHeaderGUI(UnityEditor.Editor editor)
        {
            const float buttonWidth = 150f;

            LTXAnnotationsLibrary library = LTXAnnotationsLibrary.instance;
            SerializedObject librarySerializedObject = new SerializedObject(library);
            EditorGUI.BeginChangeCheck();
            if(editor.targets.Length > 1)
                return;
            
            foreach (var t in editor.targets)
            {
                string path = AssetDatabase.GetAssetPath(t);
                if (string.IsNullOrEmpty(path))
                    continue;

                string guid = AssetDatabase.AssetPathToGUID(path);
                GUILayout.Space(20);

                if (library.Exists(guid, out var result, out int index))
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false), GUILayout.Width(buttonWidth)))
                    {
                        RemoveFolderData(library, guid);
                        return;
                    }

                    string listName = library.ListName;
                    SerializedProperty property = librarySerializedObject
                        .FindProperty(listName)
                        .GetArrayElementAtIndex(index)
                        .FindPropertyRelative(nameof(LTXProjectAnnotation.annotation));


                    SerializedProperty fontSizeProperty = property.FindPropertyRelative("fontSize");
                    SerializedProperty annotationProperty = property.FindPropertyRelative("annotation");

                    bool buttonPressed = GUILayout.Button(IsEditing ? "Save" : "Edit", GUILayout.ExpandWidth(false), GUILayout.Width(buttonWidth));
                    int fontSize = EditorGUILayout.IntSlider((int)fontSizeProperty.floatValue, 10, 30, GUILayout.ExpandWidth(true));
                    fontSizeProperty.floatValue = fontSize;

                    EditorGUILayout.EndHorizontal();

                    if (buttonPressed)
                        IsEditing = !IsEditing;

                    if (IsEditing)
                    {
                        GUIStyle style = new GUIStyle(EditorStyles.textArea);
                        style.fontSize = fontSize;
                        float height = 36f + Mathf.CeilToInt(style.CalcSize(new GUIContent(annotationProperty.stringValue)).y);
                        var temp = EditorGUILayout.TextArea(annotationProperty.stringValue, style,
                            GUILayout.Height(height));

                        annotationProperty.stringValue = temp;
                    }
                    else
                    {
                        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                        style.fontSize = fontSize;
                        style.richText = true;

                        GUILayout.Box(new GUIContent(annotationProperty.stringValue), style);
                    }
                }
                else
                {
                    if (GUILayout.Button("Add Note"))
                        AddFolderData(library, guid);
                }

            }

            if (EditorGUI.EndChangeCheck())
            {
                librarySerializedObject.ApplyModifiedProperties();
                library.Save();
            }
        }

        private static void DrawFolderIcon(string guid, Rect rect)
        {
            if(!LTXAnnotationsLibrary.instance.IsValid(guid))
                return;

            if (!LTXAnnotationsLibrary.instance.Exists(guid, out var result))
                return;

            if (!string.IsNullOrEmpty(result.annotation.Annotation))
            {
                float iconWidth = 15;
                EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
                var padding = new Vector2(5, 0);
                var iconDrawRect = new Rect(
                    rect.xMax - (iconWidth + padding.x),
                    rect.yMin,
                    rect.width,
                    rect.height);

                EditorGUI.LabelField(iconDrawRect, EditorGUIUtility.IconContent("TextAsset Icon"));
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }

        }

        internal static void AddFolderData(LTXAnnotationsLibrary library, string guid)
        {
            library.AddFolderData(new LTXProjectAnnotation() { annotation = new Annotable("Write a new note..."), guid = guid, });

            Reload();
        }

        internal static void RemoveFolderData(LTXAnnotationsLibrary library, string guid)
        {
            library.DeleteFolderData(guid, out _);
            Reload();
        }

        private static void Reload()
        {
        }
    }
}