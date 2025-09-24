using LTX.ChanneledProperties.Editor.Core;
using UnityEditor;

namespace LTX.ChanneledProperties.Editor.Conditions
{
    public class ConditionPropertyElement : ChanneledPropertyElement<bool>
    {

        public ConditionPropertyElement(IChanneledPropertyEditor<bool> channeledProperty, SerializedProperty property) : base(channeledProperty, property)
        {

        }

        protected override ChannelElement<bool>  CreateChannelElement() => new ConditionChannelElement();
    }
}