using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class DashAbilityStrategy<TData> : IAbilityStrategy<DashData, TData>
        where TData : IDashAbilityStrategyData, IAbilityStrategyData
    {
        protected PlayerMovement PlayerMovement { get; private set; }
        protected TData Data { get; private set; }
        protected IAbilityCaster Caster { get; private set; }

        protected HashSet<IDamageable> damagedEnemies;
        
        public void Initialize(IAbility<DashData> ability, IAbilityCaster caster, TData data)
        {
            if (caster.gameObject.TryGetComponent(out PlayerMovement pm))
            {
                PlayerMovement = pm;
            }

            Data = data;
            damagedEnemies = new HashSet<IDamageable>(8);
        }

        public virtual void Begin(IAbility<DashData> ability)
        {
            PlayerMovement.SetDash(true);
        }

        public virtual void Tick(IAbility<DashData> ability, float deltaTime)
        {
            // Vérifier les collisions pendant le dash
            Vector3 capsulePos = Caster.transform.position + Vector3.up * PlayerMovement.CapsuleCollider.height / 2;
    
            Collider[] colliders = Physics.OverlapCapsule(
                capsulePos,
                capsulePos + Vector3.up * PlayerMovement.CapsuleCollider.height,
                PlayerMovement.CapsuleCollider.radius + ability.Data.CastDistanceThreshold,
                GameMetrics.Global.HittableLayers,
                QueryTriggerInteraction.Collide);

            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent(out IDamageable damageable) &&
                    !damagedEnemies.Contains(damageable))
                {
                    damageable.TakeDamage(ability.Data.Damage);
                }
            }
        }

        public virtual void End(IAbility<DashData> ability)
        {
            PlayerMovement.SetDash(false);
        }

        public void Dispose(IAbility<DashData> ability)
        {
        }
    }

    [CreateStrategyFor(typeof(DashStrategyData))]
    public class AssaultDashAbilityStrategy : DashAbilityStrategy<DashStrategyData>
    {
        public override void End(IAbility<DashData> ability)
        {
            base.End(ability);
            foreach (IDamageable enemy in damagedEnemies)
            {
                enemy.TakeDamage(Data.FlameDamage);
            }
        }
    }
    
    [CreateStrategyFor(typeof(ElectricDashStrategyData))]
    public class ElectricDashAbilityStrategy : DashAbilityStrategy<ElectricDashStrategyData>
    {
        public override void End(IAbility<DashData> ability)
        {
            base.End(ability);

            // Cast for Electric Damage
            Collider[] colliders = Physics.OverlapCapsule(
                Caster.transform.position, 
                Caster.transform.forward * Data.ExplosionRadius, 
                Data.ExplosionRadius,
                GameMetrics.Global.HittableLayers, 
                QueryTriggerInteraction.Collide);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];
                if (col.gameObject.TryGetComponent(out IDamageable damageable) && 
                    col.CompareTag("Enemy"))
                {
                    damageable.TakeDamage(Data.ElectricDamage);
                }
            }
        }
    }
    
    [CreateStrategyFor(typeof(SpectralDashStrategyData))]
    public class SpectralDashAbilityStrategy : DashAbilityStrategy<SpectralDashStrategyData>
    {
        
    }
}