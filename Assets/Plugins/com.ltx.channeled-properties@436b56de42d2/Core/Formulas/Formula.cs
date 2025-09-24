using System;
using LTX.ChanneledProperties.Formulas.Controller;

namespace LTX.ChanneledProperties.Formulas
{
    /// <summary>
    /// A property with a base value that gets modified by all channels.
    /// Channels are separated in groups and ordered in them si you can group some operations and emulate parentheses.
    /// </summary>
    /// <typeparam name="T">Type of value returned</typeparam>
    [Serializable]
    public class Formula<T> : ChanneledProperty<T, FormulaChannel<T>> where T : unmanaged
    {
        public override T Value
        {
            get
            {
                if(NeedsRefresh)
                {
                    CalculateValue();
                    NeedsRefresh = false;
                }

                return value;
            }
        }

        public T StartValue => startValue;

        private bool NeedsRefresh
        {
            get => needsRefresh;
            set
            {
                needsRefresh = value;
                if (value && HasCallbacks)
                    NotifyValueChange();
            }
        }


        protected T value;
        protected T startValue;

        private bool needsRefresh;

        public Formula(T startValue, int capacity = -1, bool expandWhenFull = false) :
            this(startValue, capacity, expandWhenFull, true) { }

        private Formula(T startValue, int capacity = -1, bool expandWhenFull = false, bool needsRefresh = true) : base(capacity, expandWhenFull)
        {
            this.startValue = startValue;
            this.needsRefresh = needsRefresh;
        }

        public void AddOperation(ChannelKey key, Operator op, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0)
            => AddOperation(key, default, op, multiplyWith, group, orderInGroup);

        public bool AddOperation(ChannelKey key, T opValue = default, Operator op = Operator.Add, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0)
        {
            FormulaChannel<T> channelToAdd = new()
            {
                value = opValue,
                op = op,
                multiplyWith = multiplyWith,
                group = group,
                orderInGroup = orderInGroup,
            };

            if (Internal_AddChannel(key, channelToAdd))
            {
                NeedsRefresh = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveOperation(ChannelKey key)
        {
            bool result = Internal_RemoveChannel(key);
            if (result)
                NeedsRefresh = true;

            return result;
        }
#region Operation shortcuts
        public void Add(ChannelKey channelKey, T add, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0) =>
            AddOperation(channelKey, add, Operator.Add, multiplyWith, group, orderInGroup);
        public void Subtract(ChannelKey channelKey, T sub, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0) =>
            AddOperation(channelKey, sub, Operator.Subtract, multiplyWith, group, orderInGroup);
        public void Multiply(ChannelKey channelKey, T mult, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0) =>
            AddOperation(channelKey, mult, Operator.Multiply, multiplyWith, group, orderInGroup);
        public void Divide(ChannelKey channelKey, T div, MultiplyWith multiplyWith = MultiplyWith.Nothing, int group = 0, int orderInGroup = 0) =>
            AddOperation(channelKey, div, Operator.Divide, multiplyWith, group, orderInGroup);

#endregion

        public override bool Write(ChannelKey key, T value)
        {
            bool result = base.Write(key, value);
            if(result)
                NeedsRefresh = true;

            return result;

        }

        public bool Modify(ChannelKey key, Func<FormulaChannel<T>, FormulaChannel<T>> modification)
        {
            if (TryGetChannel(key, out FormulaChannel<T> channel))
            {
                int index = GetChannelIndex(key);
                channels[index] = ChannelContainer<FormulaChannel<T>, T>.New(key, modification(channel));
                NeedsRefresh = true;

                return true;
            }

            return false;
        }

        protected virtual T CalculateValue()
        {
            if (!CalculatorController<T>.TryGetResult(this, out value))
                value = startValue;

            return value;
        }
    }
}