using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    public class FrostBiteGrenade : IGadget
    {
        GadgetData IGadget.Data => data;

        private readonly FrostBiteGrenadeData data;
        public bool IsEquiped { get; }
        public bool IsCasting { get; }
        public event Action OnGadgetEnded;
        
        public FrostBiteGrenade(FrostBiteGrenadeData frostBiteGrenadeData)
        {
            data = frostBiteGrenadeData;
        }

        public void Begin(ICaster caster)
        {
            
        }

        public void Cast(ICaster caster)
        {
            
        }

        public void Tick(float deltaTime)
        {
            
        }

        public void End()
        {
            
        }
    }
}