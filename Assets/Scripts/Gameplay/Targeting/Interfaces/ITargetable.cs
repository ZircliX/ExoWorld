using System;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
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