using Helteix.Tools.Editor.Serialisation;
using OverBang.Pooling.Resource;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OverBang.Pooling.Editor
{
    [CustomPropertyDrawer(typeof(AddressablePoolAsset))]
    public class AddressablePoolAssetEditor : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty prop = property.FindBackingFieldPropertyRelative(nameof(AddressablePoolAsset.AssetReference));
            return new PropertyField(prop) { label = string.Empty };
        }
    }
}