using System;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class CryoExplosion : IExplosionStrategy
    {
        private readonly DamageData damage;
        private readonly float slowDuration;
        private readonly float slowPercentage;
        
        public event Action<bool> OnExploded;

        public CryoExplosion(DamageData damage, float slowDuration, float slowPercentage)
        {
            this.damage = damage;
            this.slowDuration = slowDuration;
            this.slowPercentage = slowPercentage;
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Collider[] colliders = getOverlapColliders();
            
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];
                
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage.GetRuntimeDamage());
                }
                
                if (col.TryGetComponent(out ISpeedTarget slowable))
                {
                    slowable.ApplySpeed(-slowPercentage, slowDuration, nameof(CryoExplosion));
                }
            }
            
            OnExploded?.Invoke(true);
        }
    }
}