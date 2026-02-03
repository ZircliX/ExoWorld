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
        public static IGadget CreateGadget(GadgetData gadgetData) => gadgetData
            switch
            {
                ShockGrenadeData shockGrenadeData => new ShockGrenade(shockGrenadeData),
                BurstGrenadeData burstGrenadeData => new BurstGrenade(burstGrenadeData),
                ParalixGrenadeData paralixGrenadeData => new ParalixGrenade(paralixGrenadeData),
                C4Data c4GadgetData => new C4(c4GadgetData),
                LifePulseData lifePulseData => new LifePulse(lifePulseData),
                FrostBiteGrenadeData frostBiteGrenadeData => new FrostBiteGrenade(frostBiteGrenadeData),
                _ => null
            };
    }
}