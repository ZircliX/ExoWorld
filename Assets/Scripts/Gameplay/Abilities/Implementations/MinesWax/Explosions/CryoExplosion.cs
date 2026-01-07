using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class CryoExplosion : IMineExplosionStrategy
    {
        private readonly DamageInfo damage;
        private readonly float slowDuration;
        private readonly float slowPercentage;
        
        public event Action<bool> OnExploded;

        public CryoExplosion(DamageInfo damage, float slowDuration, float slowPercentage)
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
                    damageable.TakeDamage(damage);
                }
                
                if (col.TryGetComponent(out ISlowable slowable))
                {
                    slowable.ApplySlow(slowPercentage, slowDuration);
                }
            }
            
            OnExploded?.Invoke(true);
        }
    }
}