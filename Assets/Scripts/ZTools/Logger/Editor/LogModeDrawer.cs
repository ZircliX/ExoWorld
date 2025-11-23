using UnityEditor;
using UnityEngine;
using ZTools.Core.ZTools.Core;

namespace ZTools.Logger.Editor.ZTools.Logger.Editor
{
    [CustomPropertyDrawer(typeof(LogMode))]
    public class LogModeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Use EditorGUI.MaskField to draw the flags enum
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
        }
    }
}