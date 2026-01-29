using System;

namespace OverBang.ExoWorld.Core.Abilities.Gadgets
{
    public interface IGadget
    {
        Action OnGadgetEnded { get; }
        void Initialize(GadgetData data);
        void Begin(ICaster caster);
        void Tick(float deltaTime);
        void End();
    }
}