using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor.Core
{
    [CustomPropertyDrawer(typeof(ChannelKey))]
    public class ChannelKeyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.boxedValue is ChannelKey channelKey)
            {
                ChannelKeyElement channelKeyElement = new ChannelKeyElement(channelKey);
                channelKeyElement.TrackPropertyValue(property, serializedProperty =>
                {
                    channelKeyElement.SetValue((ChannelKey)serializedProperty.boxedValue);
                });
                return channelKeyElement;
            }

            return null;
        }
    }
}