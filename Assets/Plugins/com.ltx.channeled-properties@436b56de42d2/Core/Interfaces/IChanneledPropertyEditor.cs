using System.Collections.Generic;

#if UNITY_EDITOR
namespace LTX.ChanneledProperties
{
    public interface IChanneledPropertyEditor<T>
    {
        int ChannelCount { get; }
        int Capacity { get; }

        bool HasFlexibleSize { get; }

        IEnumerable<(ChannelKey key, IChannel<T> channel)> GetChannels();
    }
}

#endif