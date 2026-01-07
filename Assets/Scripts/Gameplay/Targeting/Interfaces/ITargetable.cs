using System;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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