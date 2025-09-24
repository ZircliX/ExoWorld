using LTX.ChanneledProperties.Formulas.Interfaces;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace LTX.ChanneledProperties.Formulas.Controller
{
    public static class CalculatorController<TValue> where TValue : unmanaged
    {
        public static ICalculator<TValue> CurrentCalculator { get; internal set; }
        public static bool HasCalculator => CurrentCalculator != null;

        private static FormulaChannel<TValue>[] channelsBuffer = new FormulaChannel<TValue>[128];

        private static FormulaChannel<TValue>[] GetChannelsBuffer(int size)
        {
            var powerOfTwo = Mathf.NextPowerOfTwo(size);
            if (channelsBuffer.Length < powerOfTwo)
                channelsBuffer = new FormulaChannel<TValue>[powerOfTwo];

            return channelsBuffer;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CurrentCalculator = null;
        }

        public static void Register<TCalculator>() where TCalculator : unmanaged, ICalculator<TValue>
            => Register(new TCalculator());

        public static void Register<TCalculator>(TCalculator calculator) where TCalculator : unmanaged, ICalculator<TValue>
        {
            CurrentCalculator = calculator;

        }

        public static void Clear()
        {
            CurrentCalculator = null;
        }


        internal static bool TryGetResult(Formula<TValue> formula, out TValue result)
        {
            if (HasCalculator)
            {

                int current = 0;

                var size = formula.ChannelCount;
                FormulaChannel<TValue>[] buffer = GetChannelsBuffer(size);
                NativeArray<int> groups = new NativeArray<int>(size, Allocator.TempJob);
                NativeArray<int> orderInGroups = new NativeArray<int>(size, Allocator.TempJob);
                NativeArray<int> indices = new NativeArray<int>(size, Allocator.TempJob);
                ChannelContainer<FormulaChannel<TValue>, TValue>[] containers = formula.GetChannelsContainers();

                for (int i = 0; i < containers.Length; i++)
                {
                    var container = containers[i];
                    if(container.isAvailable)
                        continue;

                    FormulaChannel<TValue> channel = container.channel;
                    groups[current] = channel.group;
                    orderInGroups[current] = channel.orderInGroup;
                    buffer[current] = channel;
                    current++;
                }

                GetChannelsOrderJob job = new GetChannelsOrderJob()
                {
                    groups = groups,
                    orderInGroups = orderInGroups,
                    size = size,
                    indices = indices
                };
                job.Run();

                GetResult(formula, buffer, indices, out result);

                groups.Dispose();
                orderInGroups.Dispose();
                indices.Dispose();
                return true;
            }

            result = default;
            return false;
        }

        private static void GetResult(Formula<TValue> formula,  FormulaChannel<TValue>[] buffer, NativeArray<int> order, out TValue result)
        {
            int size = formula.ChannelCount;
            int currentGroup = 0;

            TValue startValue = formula.StartValue;
            TValue currentValue = startValue;
            TValue startGroupValue = startValue;
            ICalculator<TValue> calculator = CurrentCalculator;

            for (int i = 0; i < size; i++)
            {
                int index = order[i];
                var currentChannel = buffer[index];

                if (currentGroup != currentChannel.group)
                    startGroupValue = currentValue;

                currentGroup = currentChannel.group;

                TValue value = currentChannel.value;
                TValue second = currentChannel.multiplyWith switch
                {
                    MultiplyWith.StartValue => calculator.Multiply(value, startValue),
                    MultiplyWith.StartGroupValue => calculator.Multiply(value, startGroupValue),
                    MultiplyWith.CurrentValue => calculator.Multiply(value, currentValue),
                    _ => value,
                };

                currentValue = currentChannel.op switch
                {
                    Operator.Multiply => calculator.Multiply(currentValue, second),
                    Operator.Divide => calculator.Divide(currentValue, second),
                    Operator.Add => calculator.Add(currentValue, second),
                    Operator.Subtract => calculator.Subtract(currentValue, second),
                    _ => currentValue,
                };
            }

            result = currentValue;
        }
    }
}