namespace LTX.ChanneledProperties.Conditions
{
    [System.Serializable]
    public class Condition : ChanneledProperty<bool, ConditionChannel>
    {
        public override bool Value
        {
            get
            {
                for (int i = 0; i < ChannelCount; i++)
                {
                    if (!channels[i].channel.Value)
                        return false;
                }

                return true;
            }
        }

        public Condition(int capacity = -1, bool expandWhenFull = false) : base(capacity, expandWhenFull)
        {

        }

        public void AddCondition(ChannelKey channelKey, bool value = true)
        {
            bool lastValue = Value;
            ConditionChannel channel = new ConditionChannel(value);
            if (Internal_AddChannel(channelKey, channel))
            {
                if(value != lastValue)
                    NotifyValueChange();
            }
        }


        public override bool Write(ChannelKey key, bool value)
        {
            bool lastValue = Value;
            if (base.Write(key, value))
            {
                if(value != lastValue)
                    NotifyValueChange();
                return true;
            }

            return false;
        }

        public void RemoveCondition(ChannelKey channelKey)
        {
            if (Internal_RemoveChannel(channelKey))
                NotifyValueChange();

        }
    }
}