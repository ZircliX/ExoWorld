using System;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class StandardExplosion : IExplosionStrategy
    {
        private readonly DamageData damage;
        
        public event Action<bool> OnExploded;

        public StandardExplosion(DamageData damage)
        {
            this.damage = damage;
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Collider[] colliders = getOverlapColliders();
            
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];
                
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
            }
            
            OnExploded?.Invoke(true);
        }
    }
}