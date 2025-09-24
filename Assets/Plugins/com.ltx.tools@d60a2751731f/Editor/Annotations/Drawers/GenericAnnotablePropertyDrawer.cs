using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.Annotations.Drawers
{
    [CustomPropertyDrawer(typeof(Annotation<>)), System.Serializable]
    public class GenericAnnotablePropertyDrawer : AnnotablePropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = base.CreatePropertyGUI(property);
            SerializedProperty valueProperty = property.FindPropertyRelative(nameof(Annotation<object>.value));


            PropertyField propertyField = container.Q<PropertyField>("Property");
            propertyField.BindProperty(valueProperty);
            propertyField.label = property.displayName;

            return container;
        }
    }
}