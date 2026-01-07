using System;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class NovaExplosion : IMineExplosionStrategy
    {
        private readonly DamageInfo damage;
        private readonly float explosionInterval;
        private readonly int targetExplostionCount;
        
        public event Action<bool> OnExploded;

        public NovaExplosion(DamageInfo damage, float explosionInterval, int targetExplostionCount)
        {
            this.damage = damage;
            this.explosionInterval = explosionInterval;
            this.targetExplostionCount = targetExplostionCount;
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Awaitable aw = DealDamage(getOverlapColliders);
            aw.Run();
        }

        private async Awaitable DealDamage(Func<Collider[]> getOverlapColliders)
        {
            int explosionCount = 0;
            
            for (int i = 1; i < targetExplostionCount; i++)
            {
                Collider[] colliders = getOverlapColliders();
                
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (colliders[j].TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(damage);
                    }
                }

                explosionCount++;
                OnExploded?.Invoke(explosionCount == targetExplostionCount);
                
                await Awaitable.WaitForSecondsAsync(explosionInterval);
            }
        }
    }
}