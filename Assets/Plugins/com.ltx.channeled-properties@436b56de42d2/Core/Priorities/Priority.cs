
// ReSharper disable InconsistentNaming

namespace LTX.ChanneledProperties.Priorities
{

    /// <summary>
    /// A property that returns the value in the channel with the highest priority.
    /// </summary>
    /// <typeparam name="T">Type of value returned</typeparam>
    [System.Serializable]
    public class Priority<T> : ChanneledProperty<T, PriorityChannel<T>>
    {
        private PriorityChannel<T> MainChannel
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                if(HasMainChannel)
                {
                    int index = GetChannelIndex(MainChannelKey);
                    if(index != - 1)
                    {
                        return channels[index];
                    }
                }
                return default;
            }
        }

        private ChannelKey MainChannelKey
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return HasMainChannel ? _mainChannelKey : default;
            }
        }
        public bool HasMainChannel
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return _hasMainChannel;
            }
        }

        public override T Value => HasMainChannel ? MainChannel.Value : _defaultValue;


        private ChannelKey _mainChannelKey;
        private bool _hasMainChannel;
        private bool _needsRefresh;
        private T _defaultValue;

        #region Constructors

        public Priority(T defaultValue = default, int capacity = 16, bool expandWhenFull = false) : base(capacity, expandWhenFull)
        {
            _needsRefresh = true;
            _defaultValue = defaultValue;
        }

        #endregion
        public void AddPriority(ChannelKey key) => AddPriority(key, PriorityTags.None, _defaultValue);
        public void AddPriority(ChannelKey key, int priority) => AddPriority(key, priority, _defaultValue);
        public void AddPriority(ChannelKey key, PriorityTags priority) => AddPriority(key, ChannelPriorityUtility.PriorityToInt(priority), _defaultValue);
        public void AddPriority(ChannelKey key, PriorityTags priority, T value) => AddPriority(key, ChannelPriorityUtility.PriorityToInt(priority), value);

        public void AddPriority(ChannelKey key, int priority, T value)
        {
            int lastMainPriority = HasMainChannel ? MainChannel.Priority : int.MinValue;
            if(Internal_AddChannel(key, new PriorityChannel<T>(priority, value)))
            {
                ChangeChannelPriority(key, priority);

                if (lastMainPriority <= priority)
                    FindMainChannel();
                else
                    _needsRefresh = true;
            }
        }

        /// <summary>
        /// Removes completly a channel
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <returns>True if the channel existed</returns>
        public bool RemovePriority(ChannelKey key)
        {
            var lastMainChannelKey = MainChannelKey;
            bool output = Internal_RemoveChannel(key);

            if (output)
            {
                if (lastMainChannelKey.Guid == key.Guid)
                    FindMainChannel();
            }

            return output;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfully wrote</returns>
        public override bool Write(ChannelKey key, T value)
        {
            bool isMainChannel = HasMainChannel && MainChannelKey.Guid == key.Guid;

            if (!base.Write(key, value))
                return false;

            //If main channel was changed
            if (isMainChannel)
            {
                NotifyValueChange();
            }

            return true;
        }

        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfully modified</returns>
        public bool ChangeChannelPriority(ChannelKey key, int newPriority)
        {
            if (!TryGetChannel(key, out PriorityChannel<T> channel))
                return false;

            channel.Priority = newPriority;
            int mainPriority = HasMainChannel ? MainChannel.Priority : -1;

            //Updating channel inside dictionary
            var channelIndex = GetChannelIndex(key);
            PriorityChannel<T> priorityChannel = channels[channelIndex].channel;
            priorityChannel.Priority = newPriority;

            channels[channelIndex] = ChannelContainer<PriorityChannel<T>, T>.New(key,priorityChannel);
            if (IsMainChannel(key) || newPriority > 0 && newPriority > mainPriority)
            {
                FindMainChannel();
            }

            return true;

        }

        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfully modified</returns>
        public bool ChangeChannelPriority(ChannelKey key, PriorityTags newPriority) =>
            ChangeChannelPriority(key, ChannelPriorityUtility.PriorityToInt(newPriority));

        /// <summary>
        /// Remove all channels
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _mainChannelKey = default;
            _hasMainChannel = false;
            _needsRefresh = true;

            NotifyValueChange();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsMainChannel(ChannelKey key) => HasMainChannel && MainChannelKey.Guid == key.Guid;


        /// <summary>
        /// Go through all channels and find the one in control
        /// </summary>
        private void FindMainChannel()
        {
            //Not dirty anymore because the new value is re-evaluated.
            _needsRefresh = false;

            bool hasNewMainChannel = false;
            bool lastHasMainChannel = _hasMainChannel;

            if (ChannelCount == 0)
            {
                _hasMainChannel = false;
                _mainChannelKey = default;

                //Force notify because the new value is default value
                if (lastHasMainChannel)
                    NotifyValueChange();
                return;
            }

            ChannelKey mainChannelKey = _mainChannelKey;
            ChannelKey lastMainChannelKey = _mainChannelKey;

            int highestPriority = -1;
            int iteration = 0;
            int index = 0;

            while (iteration < ChannelCount)
            {
                //Skipping unused channels
                if (IsChannelAvailable(index))
                {
                    index++;
                    continue;
                }

                ChannelContainer<PriorityChannel<T>,T> container = channels[index];
                PriorityChannel<T> channel = container.channel;
                iteration++;

                int priority = channel.Priority;
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    mainChannelKey = container.key;
                    hasNewMainChannel = true;
                }

                index++;
            }

            //Channels with priority set to none can never be in control.
            //If all channels are set to none, then the property returns the default value.
            if (hasNewMainChannel)
            {
                _hasMainChannel = true;
                _mainChannelKey = mainChannelKey;
            }
            else
            {
                _hasMainChannel = false;
            }

            if (lastHasMainChannel != hasNewMainChannel || lastMainChannelKey.Guid != mainChannelKey.Guid)
                NotifyValueChange();

        }
    }
}