using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace LTX.ChanneledProperties.Editor.Core
{
    public abstract class ChannelElement<T> : VisualElement
    {
        private BaseField<T> valueField;
        private ChannelKeyElement channelKeyElement;

        internal virtual void Init(ChanneledPropertyElement<T> element, IChannel<T> channel)
        {
            valueField = CreateBaseField(channel.Value);
            VisualElement valueContainer = new();

            valueContainer.AddToClassList("channel-value");
            channelKeyElement = new ChannelKeyElement();

            valueContainer.Add(channelKeyElement);
            valueContainer.Add(valueField);

            Add(valueContainer);
        }

        internal virtual void Update(ChannelKey channelKey, IChannel<T> channel)
        {
            if(valueField != null)
                valueField.SetValueWithoutNotify(channel.Value);

            channelKeyElement.SetValue(channelKey);
        }

        internal virtual void Dispose(ChanneledPropertyElement<T> element)
        {
            Clear();
        }

        protected static BaseField<T> CreateBaseField<TValue>(TValue value)
        {
            VisualElement field;

            switch (value)
            {
                case Object obj:
                    field = new ObjectField() { objectType = value.GetType(), value = obj, };
                    break;
                case Enum e:
                    field = e.GetType().GetCustomAttributes(true).Any(ctx => ctx is FlagsAttribute)
                        ? new EnumFlagsField(e.ToString(), e) { value = e, }
                        : new EnumField(e.ToString(), e) { value = e, };
                    break;
                case string s:
                    field = new TextField() { value = s };
                    break;
                case int i:
                    field = new IntegerField() { value = i };
                    break;
                case float f:
                    field = new FloatField() { value = f };
                    break;
                case double d:
                    field = new DoubleField() { value = d };
                    break;
                case long l:
                    field = new LongField() { value = l };
                    break;
                case bool b:
                    field = new Toggle() { value = b };
                    break;
                case AnimationCurve curve:
                    field = new CurveField() { value = curve };
                    break;
                case LayerMask mask:
                    field = new LayerMaskField() { value = mask };
                    break;
                case Gradient gradient:
                    field = new GradientField() { value = gradient };
                    break;
                case Vector3 v:
                    field = new Vector3Field() { value = v };
                    break;
                case Vector2 v2:
                    field = new Vector2Field() { value = v2 };
                    break;
                case Vector4 v4:
                    field = new Vector4Field() { value = v4 };
                    break;
                case Rect rect:
                    field = new RectField() { value = rect };
                    break;
                case Bounds bounds:
                    field = new BoundsField() { value = bounds };
                    break;
                case BoundsInt boundsInt:
                    field = new BoundsIntField() { value = boundsInt };
                    break;
                case Color color:
                    field = new ColorField() { value = color };
                    break;
                default:
                    field = null;
                    break;
            }
            if(field!= null)
                field.AddToClassList("channel-value-field");
            return field as BaseField<T>;
        }
    }
}