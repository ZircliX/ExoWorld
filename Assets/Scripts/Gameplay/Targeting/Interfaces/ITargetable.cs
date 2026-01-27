using System;
using OverBang.ExoWorld.Core.Interactions;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface ITargetable
    {
        event Action<bool> OnTargetableChanged;
        Transform Transform { get; }
        TargetPriority Priority { get; }
        bool IsTargetable { get; }

        void SetTargetable(bool state);
    }
}