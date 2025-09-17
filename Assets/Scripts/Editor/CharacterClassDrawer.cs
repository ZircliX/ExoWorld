using OverBang.GameName.Core.Characters;
using UnityEditor;
using UnityEngine;

namespace OverBang.GameName.Editor
{
    [CustomPropertyDrawer(typeof(CharacterClasses))]
    public class CharacterClassDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Current value as enum
            CharacterClasses currentValue = (CharacterClasses)property.intValue;

            // Show the mask field
            CharacterClasses newValue = (CharacterClasses)EditorGUI.EnumFlagsField(position, label, currentValue);

            // Assign back the correct enum mask
            property.intValue = (int)newValue;
        }
    }
}