using System;
using DamageNumbersPro;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class CryoExplosion : IExplosionStrategy, IDamageSource
    {
        private readonly DamageData damage;
        private readonly float slowDuration;
        private readonly float slowPercentage;
        private readonly DamageNumberMesh damagePrefab;
        
        public event Action<bool> OnExploded;

        public CryoExplosion(DamageData damage, float slowDuration, float slowPercentage, DamageNumberMesh damagePrefab)
        {
            this.damage = damage;
            this.slowDuration = slowDuration;
            this.slowPercentage = slowPercentage;
            this.damagePrefab = damagePrefab;
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            Collider[] colliders = getOverlapColliders();
            
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];
                
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    Damage(damageable);
                }
                
                if (col.TryGetComponent(out ISpeedTarget slowable))
                {
                    slowable.ApplySpeed(-slowPercentage, slowDuration, nameof(CryoExplosion));
                }
            }
            
            OnExploded?.Invoke(true);
        }
        
        public DamageData DamageData => damage;
        public void Damage(IDamageable damageable)
        {
            RuntimeDamageData runtimeDamageData = damage.GetRuntimeDamage();
            
            damageable.TakeDamage(runtimeDamageData);
            damagePrefab.Spawn(damageable.DamageTarget.position, runtimeDamageData.finalDamage, damageable.DamageTarget);
        }
    }
}