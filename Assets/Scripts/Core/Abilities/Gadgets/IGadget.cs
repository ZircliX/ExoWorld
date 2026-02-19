using System;

namespace OverBang.ExoWorld.Core.Abilities.Gadgets
{
    public interface IGadget
    {
        bool IsEquiped { get; }
        bool IsCasting { get; }
        event Action<IGadget> OnGadgetCasted;
        event Action<IGadget> OnGadgetEnded;
        
        
        GadgetData Data { get; }
        void Begin(ICaster caster);
        void Cast(ICaster caster);
        void Tick(float deltaTime);
        void End();
        void Discard();
    }
}