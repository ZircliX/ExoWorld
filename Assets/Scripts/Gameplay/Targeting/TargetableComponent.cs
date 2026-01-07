using System;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class TargetableComponent : MonoBehaviour, ITargetable
    {
        public event Action<bool> OnTargetableChanged;

        public Transform Transform => transform;
        public TargetPriority Priority { get; private set; } = TargetPriority.Medium;
        public bool IsTargetable { get; private set; } = true;
        
        public void SetTargetable(bool state)
        {
            IsTargetable = state;
            OnTargetableChanged?.Invoke(state);
        }
    }
}