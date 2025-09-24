using LTX.ChanneledProperties.Editor.Core;
using UnityEditor;

namespace LTX.ChanneledProperties.Editor.Priorities
{
    public class PriorityPropertyElement<T> : ChanneledPropertyElement<T>
    {
        public PriorityPropertyElement(IChanneledPropertyEditor<T> channeledProperty, SerializedProperty property)
            : base(channeledProperty, property)
        {

        }

        protected override ChannelElement<T>  CreateChannelElement() => new PriorityChannelElement<T>();
    }
}