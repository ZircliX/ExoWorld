using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Gameplay.Loadout.BurstGadget;
using OverBang.ExoWorld.Gameplay.Loadout.C4Gadget;
using OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget;
using OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget;
using OverBang.ExoWorld.Gameplay.Loadout.ParalixGadget;
using OverBang.ExoWorld.Gameplay.Loadout.ShockGadget;


namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public static class GadgetFactory
    {
        public static IGadget CreateGadget(GadgetData gadgetData)
        {
            switch (gadgetData.GetType())
            {
                case { } t when t == typeof(ShockGrenadeData):
                    return new ShockGrenade();
                
                case { } t when t == typeof(BurstGrenadeData):
                    return new BurstGrenade();
                
                case { } t when t == typeof(ParalixGrenadeData):
                    return new ParalixGrenade();
                
                case { } t when t == typeof(C4Data):
                    return new C4();
                
                case { } t when t == typeof(LifePulseData):
                    return new LifePulse();
                
                case { } t when t == typeof(FrostBiteGrenadeData):
                    return new FrostBiteGrenade();
                
                default:
                    return null;
            }
        }
    }
}