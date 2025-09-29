using Helteix.Tools.Editor.Serialisation;
using OverBang.Pooling.Resource;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OverBang.Pooling.Editor
{
    [CustomPropertyDrawer(typeof(ResourcePoolAsset))]
    public class ResourcePoolAssetEditor : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty prop = property.FindBackingFieldPropertyRelative(nameof(ResourcePoolAsset.Path));
            return new PropertyField(prop) { label = string.Empty };
        }
    }
}