using System;
using DamageNumbersPro;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class StandardExplosion : IExplosionStrategy, IDamageSource
    {
        private readonly DamageData damage;
        private readonly DamageNumberMesh damagePrefab;
        
        public event Action<bool> OnExploded;

        public StandardExplosion(DamageData damage, DamageNumberMesh damagePrefab)
        {
            this.damage = damage;
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