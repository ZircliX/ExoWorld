using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;

namespace OverBang.ExoWorld.Gameplay.Loadout.C4Gadget
{
    public class C4 : IGadget
    {
        GadgetData IGadget.Data => data;
        
        private readonly C4Data data;
        public bool IsEquiped { get; }
        public bool IsCasting { get; }
        public event Action OnGadgetDiscarded;
        public event Action<IGadget> OnGadgetCasted;
        public event Action<IGadget> OnGadgetEnded;

        public C4(C4Data c4GadgetData)
        {
            this.data = c4GadgetData;
        }

        
        
        public void Begin(ICaster caster)
        {
            throw new NotImplementedException();
        }

        public void Cast(ICaster caster)
        {
            throw new NotImplementedException();
        }

        public void Tick(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Discard()
        {
            throw new NotImplementedException();
        }
    }
}