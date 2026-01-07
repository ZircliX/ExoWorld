using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class StandardExplosion : IMineExplosionStrategy
    {
        private readonly DamageInfo damage;
        
        public event Action<bool> OnExploded;

        public StandardExplosion(DamageInfo damage)
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