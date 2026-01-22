using System;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class NovaExplosion : IExplosionStrategy
    {
        private readonly DamageInfo damage;
        private readonly float explosionInterval;
        private readonly int targetExplosionCount;
        
        public event Action<bool> OnExploded;

        public NovaExplosion(DamageInfo damage, float explosionInterval, int targetExplosionCount)
        {
            this.damage = damage;
            this.explosionInterval = explosionInterval;
            this.targetExplosionCount = targetExplosionCount;
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Awaitable aw = DealDamage(getOverlapColliders);
            aw.Run();
        }

        private async Awaitable DealDamage(Func<Collider[]> getOverlapColliders)
        {
            int explosionCount = 0;
            
            for (int i = 0; i < targetExplosionCount; i++)
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
                OnExploded?.Invoke(explosionCount == targetExplosionCount);
                
                await Awaitable.WaitForSecondsAsync(explosionInterval);
            }
            
            Debug.Log(explosionCount);
        }
    }
}