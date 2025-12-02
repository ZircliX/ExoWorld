using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class HealableComponent : MonoBehaviour, IHealable
    {
        public event Action OnHealed;
        
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > 0;
        
        public void Heal(float amount)
        {
            Health += amount;
            OnHealed?.Invoke();
        }
    }
}