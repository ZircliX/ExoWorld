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
                    ShockGrenade shockGrenade = new ShockGrenade();
                    shockGrenade.Initialize(gadgetData);
                    return shockGrenade;
                
                case { } t when t == typeof(BurstGrenadeData):
                    BurstGrenade burstGrenade = new BurstGrenade();
                    burstGrenade.Initialize(gadgetData);
                    return burstGrenade;
                
                case { } t when t == typeof(ParalixGrenadeData):
                    ParalixGrenade paralixGrenade = new ParalixGrenade();
                    paralixGrenade.Initialize(gadgetData);
                    return paralixGrenade;
                
                case { } t when t == typeof(C4Data):
                    C4 c4 = new C4();
                    c4.Initialize(gadgetData);
                    return c4;
                
                case { } t when t == typeof(LifePulseData):
                    LifePulse lifePulse = new LifePulse();
                    lifePulse.Initialize(gadgetData);
                    return lifePulse;
                
                case { } t when t == typeof(FrostBiteGrenadeData):
                    FrostBiteGrenade frostBiteGrenade = new FrostBiteGrenade();
                    frostBiteGrenade.Initialize(gadgetData);
                    return frostBiteGrenade;
                
                default:
                    return null;
            }
        }
    }
}