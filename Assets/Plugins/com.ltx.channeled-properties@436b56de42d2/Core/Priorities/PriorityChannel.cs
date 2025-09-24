namespace LTX.ChanneledProperties.Priorities
{
    public struct PriorityChannel<T> : IChannel<T>
    {
        T IChannel<T>.Value { get => Value; set => this.Value = value; }

        public int Priority { get; internal set; }
        public T Value { get; internal set; }

        internal PriorityChannel(int priority, T value)
        {
            Priority = priority;
            Value = value;
        }

        internal PriorityChannel(PriorityTags priority, T value) :
            this(ChannelPriorityUtility.PriorityToInt(priority), value)
        {

        }
    }
}