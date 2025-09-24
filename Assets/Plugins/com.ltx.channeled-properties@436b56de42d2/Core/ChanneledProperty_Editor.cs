
#if UNITY_EDITOR
using System.Collections.Generic;

namespace LTX.ChanneledProperties
{
    /// <summary>
    /// Base class
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TChannel"></typeparam>
    public abstract partial class ChanneledProperty<TValue, TChannel> : IChanneledPropertyEditor<TValue>
        where TChannel : IChannel<TValue>
    {
        bool IChanneledPropertyEditor<TValue>.HasFlexibleSize => expandWhenFull;

        IEnumerable<(ChannelKey key, IChannel<TValue> channel)> IChanneledPropertyEditor<TValue>.GetChannels()
        {
            if(channels == null)
                yield break;

            foreach (var container in channels)
            {
                if (!container.isAvailable)
                    yield return (container.key, container.channel);
            }
        }
    }
}
#endif