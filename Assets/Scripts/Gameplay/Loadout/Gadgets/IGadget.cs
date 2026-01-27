using System;
using OverBang.ExoWorld.Gameplay.Abilities;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IGadget
    {
        ICaster Caster { get; }
        GadgetData Data { get; }
        
        Action OnGadgetEnded { get; }
        void Begin();
        void Tick(float deltaTime);
        void End();
    }

    public interface IGadget<TData> : IGadget where TData : GadgetData
    {
        TData DataT { get; }
        void Initialize(TData data, ICaster caster);
        
    }
}