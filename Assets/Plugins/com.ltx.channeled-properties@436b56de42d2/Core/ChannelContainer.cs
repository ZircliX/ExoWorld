namespace LTX.ChanneledProperties
{
    public readonly struct ChannelContainer<TChannel, TValue> where TChannel : IChannel<TValue>
    {
        public readonly ChannelKey key;
        public readonly TChannel channel;
        public readonly bool isAvailable;

        public static ChannelContainer<TChannel, TValue> Empty() => new(ChannelKey.None, default, true);
        public static ChannelContainer<TChannel, TValue> New(ChannelKey key, TChannel channel) => new(key, channel, false);

        private ChannelContainer(ChannelKey key, TChannel channel, bool isAvailable)
        {
            this.key = key;
            this.channel = channel;
            this.isAvailable = isAvailable;
        }

        public static implicit operator TChannel(ChannelContainer<TChannel, TValue> container) => container.channel;
    }

    public interface IChannelContainer
    {
        public ChannelKey key { get; }
        public bool isAvailable{ get; }
    }
}