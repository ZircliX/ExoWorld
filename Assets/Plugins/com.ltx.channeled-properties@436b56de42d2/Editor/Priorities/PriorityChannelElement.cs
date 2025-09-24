using LTX.ChanneledProperties.Editor.Core;
using LTX.ChanneledProperties.Priorities;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor.Priorities
{
    public class PriorityChannelElement<T> : ChannelElement<T>
    {
        private IntegerField priorityField;

        internal override void Init(ChanneledPropertyElement<T> element, IChannel<T> channel)
        {
            base.Init(element, channel);
            priorityField = new IntegerField("Priority");

            Add(priorityField);
        }

        internal override void Update(ChannelKey channelKey, IChannel<T> channel)
        {
            base.Update(channelKey, channel);
            if (channel is PriorityChannel<T> priorityChannel)
            {
                priorityField.SetValueWithoutNotify(priorityChannel.Priority);
            }
        }
    }
}