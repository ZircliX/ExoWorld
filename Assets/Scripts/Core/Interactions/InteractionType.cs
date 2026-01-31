using System;

namespace OverBang.ExoWorld.Core.Interactions
{
    [Flags]
    public enum InteractionType
    {
        None = 0,
        Interact = 1,
        Pickup = 2,
        Drop = 4
    }
}