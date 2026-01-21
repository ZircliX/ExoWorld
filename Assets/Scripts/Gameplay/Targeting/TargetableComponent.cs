using System;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
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