using System;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class TargetableComponent : MonoBehaviour, ITargetable
    {
        public event Action OnTargeted;
        
        public Transform Transform { get; }
        public TargetPriority Priority { get; }
        public bool IsTargetable { get; }
        
        public void Target()
        {
            OnTargeted?.Invoke();
        }
    }
}