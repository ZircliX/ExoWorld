namespace LTX.ChanneledProperties.Priorities
{
    public enum PriorityTags
    {
        None = -1,
        Smallest = 1,
        VerySmall = 5,
        Small = 15,
        Default = 30,
        High = 50,
        VeryHigh = 100,
        Highest = 300
    }
    internal static class ChannelPriorityUtility
    {
        
        internal static int PriorityToInt(PriorityTags priorityTags) => priorityTags switch
        {
            PriorityTags.None => -1,
            PriorityTags.Smallest => 1,
            PriorityTags.VerySmall => 3,
            PriorityTags.Small => 5,
            PriorityTags.Default => 10,
            PriorityTags.High => 25,
            PriorityTags.VeryHigh => 50,
            PriorityTags.Highest => 100,
            _ => -1,
        };

    }
}
