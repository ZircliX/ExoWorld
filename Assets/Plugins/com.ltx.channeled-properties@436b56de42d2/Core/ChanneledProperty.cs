using System;
using System.Collections.Generic;
using LTX.ChanneledProperties.Settings;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    [Serializable]
    public abstract partial class ChanneledProperty<TValue, TChannel>
        where TChannel : IChannel<TValue>
    {
        public event Action<TValue> OnValueChanged;

        protected bool HasCallbacks => OnValueChanged != null;

        //Properties

        public abstract TValue Value { get; }
        public int ChannelCount { get; private set; }
        public int Capacity { get; private set; }

        public readonly bool expandWhenFull;

        protected ChannelContainer<TChannel, TValue>[] channels;
        protected readonly Dictionary<ChannelKey, int> keyPointers;

        protected ChanneledProperty()
        {

        }

        protected ChanneledProperty(int capacity, bool expandWhenFull)
        {
            ChanneledPropertiesSettings settings = ChanneledPropertiesSettings.Current;
            capacity = capacity <= 0 ? settings.DefaultCapacity : capacity;

            if (settings.ForceCapacityToNextPowerOfTwo)
                capacity = Mathf.NextPowerOfTwo(capacity);

            this.expandWhenFull = expandWhenFull;

            Capacity = capacity;
            channels = new ChannelContainer<TChannel, TValue>[Capacity];
            keyPointers = new Dictionary<ChannelKey, int>(Capacity);

            for (int i = 0; i < capacity; i++)
                channels[i] = ChannelContainer<TChannel, TValue>.Empty();
        }

        public TValue this[ChannelKey key]
        {
            get => GetValueFrom(key);
            set => Write(key, value);
        }


        protected int GetAvailableSlot()
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (channels[i].isAvailable)
                    return i;
            }

            return -1;
        }

        protected bool Internal_AddChannel(ChannelKey channelKey, TChannel channel)
        {
            if (!InternalCanAddChannel(channelKey))
                return false;

            int index = GetAvailableSlot();
            if (index == -1)
                return false;

            if (channels[index].isAvailable)
            {
                channels[index] = ChannelContainer<TChannel, TValue>.New(channelKey, channel);

                keyPointers.Add(channelKey, index);
                ChannelCount++;
                return true;
            }

            return false;
        }


        private bool InternalCanAddChannel(ChannelKey key)
        {
            if (HasChannel(key))
                return false;

            if (ChannelCount >= Capacity)
            {
                //If here, then the channeled property has reached max capacity
                if (expandWhenFull)
                    ExpandChannelsBuffer();
                else
                {
                    Debug.LogError($"Couldn't add channel. ChanneledProperty has reached maximum size. Consider changing the capacity or reducing the concurrent usage.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes a channel
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <returns>True if the channel existed</returns>
        protected bool Internal_RemoveChannel(ChannelKey key)
        {
            if (HasChannel(key))
            {
                int index = keyPointers[key];

                channels[index] = ChannelContainer<TChannel, TValue>.Empty();
                keyPointers.Remove(key);


                ChannelCount--;
                return true;
            }

            return false;
        }

        private void ExpandChannelsBuffer()
        {
            int newCapacity = Mathf.NextPowerOfTwo(Mathf.IsPowerOfTwo(Capacity) ? Capacity + 1 : Capacity);
            //TODO use static buffer
            ChannelContainer<TChannel, TValue>[] newChannels = new ChannelContainer<TChannel, TValue>[Capacity];

            for (int i = 0; i < newCapacity; i++)
            {
                var exists = i < Capacity;
                ChannelContainer<TChannel, TValue> container = exists && !channels[i].isAvailable?
                    ChannelContainer<TChannel, TValue>.New(channels[i].key, channels[i].channel) :
                    ChannelContainer<TChannel, TValue>.Empty();

                newChannels[i] = container;
            }

            Capacity = newCapacity;
            channels = new ChannelContainer<TChannel, TValue>[Capacity];

            for( int i = 0;i < newCapacity; i++)
                channels[i] = newChannels[i];
        }


        /// <summary>
        /// Get the channel of a key if he's in charge of it.
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <param name="channel">Output. Default if not found.</param>
        /// <returns>True if a channel is found</returns>
        public bool TryGetChannel(ChannelKey key, out TChannel channel)
        {
            if (keyPointers.TryGetValue(key, out int index))
            {
                channel = channels[index].channel;
                return true;
            }
            else
            {
                channel = default;
                return false;
            }
        }

        public TValue GetValue(ChannelKey key) => channels[GetChannelIndex(key)].channel.Value;
        public bool TryGetValue(ChannelKey key, out TValue value)
        {
            if (TryGetChannel(key, out TChannel channel))
            {
                value = channel.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfully wrote</returns>
        public virtual bool Write(ChannelKey key, TValue value)
        {
            if (!TryGetChannel(key, out TChannel channel))
                return false;

            channel.Value = value;
            //Updating struct value inside dictionary
            int index = keyPointers[key];
            channels[index] = ChannelContainer<TChannel, TValue>.New(key, channel);

            return true;
        }

        /// <summary>
        /// Remove all channels
        /// </summary>
        public virtual void Clear()
        {
            keyPointers.Clear();

            for (int i = 0; i < Capacity; i++)
            {
                channels[i] = ChannelContainer<TChannel, TValue>.Empty();
            }

            ChannelCount = 0;
        }

        public void AddOnValueChangeCallback(Action<TValue> callback, bool callImmediate = false)
        {
            OnValueChanged += callback;
            if(callImmediate)
                callback?.Invoke(Value);
        }
        public void RemoveOnValueChangeCallback(Action<TValue> callback, bool callImmediate = false)
        {
            OnValueChanged -= callback;
            if(callImmediate)
                callback?.Invoke(Value);
        }

        public void NotifyValueChange() => NotifyValueChange(Value);
        protected virtual void NotifyValueChange(TValue value)
        {
            OnValueChanged?.Invoke(value);
        }

        #region Utility
        /// <summary>
        /// Faster way to get value from key but key needs to exists
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <returns>Value of channel</returns>
        private TValue GetValueFrom(ChannelKey key)
        {
            if (TryGetChannel(key, out TChannel channel))
                return channel.Value;

            return default(TValue);
        }

        /// <summary>
        /// Does this key is in charge of the main channel?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool HasChannel(ChannelKey key) => keyPointers.ContainsKey(key);

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected int GetChannelIndex(ChannelKey key) => HasChannel(key) ? keyPointers[key] : -1;

        protected bool IsChannelAvailable(int index) => index >= 0 && index < Capacity && channels[index].isAvailable;

        #endregion

        public static implicit operator TValue(ChanneledProperty<TValue,TChannel> cp) => cp == null ? default : cp.Value;


        public override string ToString()
        {
            return Value.ToString();
        }
        internal ChannelContainer<TChannel,TValue>[]  GetChannelsContainers() => channels;
    }
}