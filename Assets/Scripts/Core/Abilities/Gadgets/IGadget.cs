using System;

namespace OverBang.ExoWorld.Core.Abilities.Gadgets
{
    public interface IGadget
    {
        bool IsEquiped { get; }
        bool IsCasting { get; }
        event Action OnGadgetEnded;
        event Action OnGadgetBeingCasted;
        
        
        GadgetData Data { get; }
        void Begin(ICaster caster);
        void Cast(ICaster caster);
        void Tick(float deltaTime);
        void End();
        void Discard();
    }
}