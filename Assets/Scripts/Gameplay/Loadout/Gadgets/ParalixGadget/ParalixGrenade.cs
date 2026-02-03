using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;

namespace OverBang.ExoWorld.Gameplay.Loadout.ParalixGadget
{
    public class ParalixGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;
        
        public ParalixGrenadeData Data { get; private set; }
        
        public bool IsEquiped { get; }
        public bool IsCasting { get; }
        public event Action OnGadgetEnded;
        
        public ParalixGrenade(ParalixGrenadeData paralixGrenadeData)
        {
            Data = paralixGrenadeData;
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