using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace LTX.ChanneledProperties.Formulas.Controller
{
    [BurstCompile(
        OptimizeFor = OptimizeFor.Performance,
        FloatPrecision = FloatPrecision.Low,
        FloatMode = FloatMode.Fast,
        CompileSynchronously = true)]
    public struct GetChannelsOrderJob : IJob
    {
        public int size;

        [ReadOnly]
        public NativeArray<int> groups;
        [ReadOnly]
        public NativeArray<int> orderInGroups;

        [WriteOnly]
        public NativeArray<int> indices;

        public void Execute()
        {
            int currentGroup = 0;
            int currentOrderInLayer = 0;

            for (int i = 0; i < size; i++)
            {
                int index = 0;
                int closestGroup = int.MaxValue;
                int closestOrderInLayer = int.MaxValue;

                for (int j = 0; j < size; j++)
                {
                    int channelGroup = groups[j];
                    int channelOrderInLayer = orderInGroups[j];

                    //Already passed group
                    if (channelGroup < currentGroup)
                        continue;

                    if (channelGroup == currentGroup)
                    {
                        if (channelOrderInLayer <= currentOrderInLayer)
                            continue;
                        if (channelOrderInLayer >= closestOrderInLayer)
                            continue;

                        index = j;
                        closestGroup = channelGroup;
                        closestOrderInLayer = channelOrderInLayer;
                    }
                    else if (channelGroup < closestGroup)
                    {
                        index = j;
                        closestGroup = channelGroup;
                        closestOrderInLayer = channelOrderInLayer;
                    }
                }

                currentGroup = closestGroup;
                currentOrderInLayer = closestOrderInLayer;
                indices[i] = index;
            }
        }
    }
}