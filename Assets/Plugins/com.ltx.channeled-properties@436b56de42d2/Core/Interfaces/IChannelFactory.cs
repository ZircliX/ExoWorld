namespace LTX.ChanneledProperties
{
    public interface IChannelFactory<TValue, TChannel> where TChannel : IChannel<TValue>
    {
        TChannel GetEmptyChannel();
    }
}