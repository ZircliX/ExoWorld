using LTX.ChanneledProperties.Editor.Core;
using UnityEditor;

namespace LTX.ChanneledProperties.Editor.Conditions
{
    [CustomPropertyDrawer(typeof(ChanneledProperties.Conditions.Condition))]
    public class ConditionPropertyDrawer : ChanneledPropertyDrawer
    {
        protected override ChanneledPropertyElement<TValue> GetElement<TValue>(SerializedProperty property, IChanneledPropertyEditor<TValue> channeledProperty)
        {
            return  new ConditionPropertyElement((IChanneledPropertyEditor<bool>)channeledProperty, property) as ChanneledPropertyElement<TValue>;
        }
    }
}