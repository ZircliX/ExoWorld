using LTX.Tools.Editor.Common;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor.Core
{
    public class ChannelKeyElement : VisualElement
    {
        internal ChannelKey channelKey;

        private Label keyText;
        private ObjectField keyObject;

        public ChannelKeyElement(ChannelKey channelKey) : this()
        {
            SetValue(channelKey);
        }

        public ChannelKeyElement()
        {
            keyText = new Label();
            keyObject = new ObjectField();

            Add(keyText);
            Add(keyObject);
        }

        public void SetValue(ChannelKey newKey)
        {
            channelKey = newKey;
            if (newKey.Source != null)
            {
                keyObject.objectType = typeof(UnityEngine.Object);
                keyObject.value = newKey.Source;

                keyObject.ShowManually();
                keyText.HideManually();
            }
            else if (!string.IsNullOrEmpty(newKey.PointerTag))
            {
                keyText.text = newKey.PointerTag;
                keyObject.HideManually();
                keyText.ShowManually();
            }
            else
            {
                keyText.text = newKey.Guid.ToString();
                keyObject.HideManually();
                keyText.ShowManually();
            }
        }
    }
}