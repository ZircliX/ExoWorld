using System;
using OverBang.ExoWorld.Core.GameMode.Players;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Abilities.Gadgets
{
    public interface IGadget
    {
        bool IsEquiped { get; }
        bool IsCasting { get; }
        event Action<IGadget> OnGadgetEnded;
        
        
        GadgetData Data { get; }
        void Begin(ICaster caster, LocalGamePlayer player);
        void Cast(Camera cam);
        void Tick(float deltaTime);
        void End();
        void Discard();
    }
}