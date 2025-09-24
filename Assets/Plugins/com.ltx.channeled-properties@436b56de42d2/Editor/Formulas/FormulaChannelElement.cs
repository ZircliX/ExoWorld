using LTX.ChanneledProperties.Editor.Core;
using LTX.ChanneledProperties.Formulas;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor.Formulas
{
    public class FormulaChannelElement<T> : ChannelElement<T> where T : unmanaged
    {
        private EnumField operatorField;
        private EnumField multiplyWithField;
        private IntegerField groupField;
        private IntegerField orderInGroupField;

        internal override void Init(ChanneledPropertyElement<T> element, IChannel<T> channel)
        {
            base.Init(element, channel);
            operatorField = new EnumField("Operator", Operator.Add);
            multiplyWithField = new EnumField("MultiplyWith", MultiplyWith.Nothing);
            groupField = new IntegerField("Group");
            orderInGroupField = new IntegerField("Order In Group");

            Add(operatorField);
            Add(multiplyWithField);
            Add(groupField);
            Add(orderInGroupField);
        }

        internal override void Update(ChannelKey channelKey, IChannel<T> channel)
        {
            base.Update(channelKey, channel);
            if (channel is FormulaChannel<T> formulaChannel)
            {
                operatorField.SetValueWithoutNotify(formulaChannel.op);
                multiplyWithField.SetValueWithoutNotify(formulaChannel.multiplyWith);
                groupField.SetValueWithoutNotify(formulaChannel.group);
                orderInGroupField.SetValueWithoutNotify(formulaChannel.orderInGroup);
            }
        }
    }
}