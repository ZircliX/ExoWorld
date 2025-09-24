namespace LTX.ChanneledProperties.Conditions
{
    public struct ConditionChannel : IChannel<bool>
    {
        public bool Value { get; internal set; }
        bool IChannel<bool>.Value { get => Value; set => this.Value = value; }

        internal ConditionChannel(bool value)
        {
            this.Value = value;
        }


        public static implicit operator bool(ConditionChannel channel) => channel.Value;
    }
}