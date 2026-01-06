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
        public bool IsTargetable => isTargetable;
        private bool isTargetable = true;
        
        public void Target()
        {
            OnTargeted?.Invoke();
        }

        public void SetTargetable(bool state)
        {
            isTargetable = state;
        }
    }
}