using System;
using OverBang.ExoWorld.Gameplay.Player;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class HealingExplosion : IExplosionStrategy
    {
        private readonly float healingValue;
        
        public event Action<bool> OnExploded;
        
        public HealingExplosion(float healingValue)
        {
            this.healingValue = healingValue;
        }
        
        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Collider[] colliders = getOverlapColliders();
            
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];
                
                if (col.TryGetComponent(out PlayerEntity playerEntity))
                {
                    playerEntity.Heal(healingValue);
                }
            }
        }
    }
}