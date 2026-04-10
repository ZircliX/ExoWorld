using System;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class Target : MonoBehaviour, IDamageable, ITargetable
    {
        [field: SerializeField] public Transform DamageTarget { get; private set; }
        public void TakeDamage(RuntimeDamageData damage)
        {
        }

        public event Action<bool> OnTargeted;
        public TargetPriority Priority { get; private set; } = TargetPriority.High;
        public bool IsTargetable { get; private set; }
        public void SetTargetable(bool state)
        {
        }
    }
}