using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    public class LifePulse : IGadget
    {
        GadgetData IGadget.Data => data;
        private readonly LifePulseData data;
        public bool IsEquiped { get; }
        public bool IsCasting { get; }
        public event Action OnGadgetEnded;

        public LifePulse(LifePulseData lifePulseData)
        {
            data = lifePulseData;
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