using System;
using LTX.ChanneledProperties.Editor.Core;
using LTX.ChanneledProperties.Priorities;
using UnityEditor;

namespace LTX.ChanneledProperties.Editor.Priorities
{

    [CustomPropertyDrawer(typeof(Priority<>))]
    public class PriorityPropertyDrawer : ChanneledPropertyDrawer
    {
        protected override ChanneledPropertyElement<TValue> GetElement<TValue>(SerializedProperty property, IChanneledPropertyEditor<TValue> channeledProperty)
        {
            Type t = typeof(PriorityPropertyElement<>).MakeGenericType(typeof(TValue));
            object a = Activator.CreateInstance(t, channeledProperty, property);

            return  a as ChanneledPropertyElement<TValue>;
        }
    }
}