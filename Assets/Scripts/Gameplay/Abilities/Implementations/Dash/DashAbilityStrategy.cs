using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Movement;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class DashAbilityStrategy<TData> : IAbilityStrategy<DashData, TData>
        where TData : IDashAbilityStrategyData, IAbilityStrategyData
    {
        protected PlayerMovement PlayerMovement { get; private set; }
        protected TData Data { get; private set; }
        protected ICaster Caster { get; private set; }

        protected HashSet<IDamageable> damagedEnemies;
        protected Collider[] bumpColliders;
        
        public void Initialize(IAbility<DashData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            
            if (caster.gameObject.TryGetComponent(out PlayerMovement pm))
            {
                PlayerMovement = pm;
            }

            Data = data;
            damagedEnemies = new HashSet<IDamageable>(8);
            bumpColliders = new Collider[8];
        }

        public virtual void Begin(IAbility<DashData> ability)
        {
            PlayerMovement.SetDash(true);
        }

        public virtual void Tick(IAbility<DashData> ability, float deltaTime)
        {
            Vector3 capsulePos = Caster.transform.position + Vector3.up * PlayerMovement.CapsuleCollider.height / 2;

            int size = Physics.OverlapCapsuleNonAlloc(capsulePos,
                capsulePos + Vector3.up * PlayerMovement.CapsuleCollider.height,
                PlayerMovement.CapsuleCollider.radius + ability.DataT.CastDistanceThreshold,
                bumpColliders,
                GameMetrics.Global.HittableLayers,
                QueryTriggerInteraction.Collide);

            for (int index = 0; index < size; index++)
            {
                Collider col = bumpColliders[index];
                
                if (col.TryGetComponent(out IDamageable damageable) &&
                    !damagedEnemies.Contains(damageable) &&
                    col.gameObject != Caster.gameObject)
                {
                    damageable.TakeDamage(ability.DataT.Damage);
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
            
            //Flame Damages
            foreach (IDamageable enemy in damagedEnemies)
            {
                enemy.TakeDamage(Data.FlameDamage);
            }
        }
    }
    
    [CreateStrategyFor(typeof(ElectricDashStrategyData))]
    public class ElectricDashAbilityStrategy : DashAbilityStrategy<ElectricDashStrategyData>
    {
        private Collider[] colliders;

        public override void Begin(IAbility<DashData> ability)
        {
            base.Begin(ability);
            colliders = new Collider[8];
        }

        public override void End(IAbility<DashData> ability)
        {
            base.End(ability);

            // Cast for Electric Damage
            int size = Physics.OverlapCapsuleNonAlloc(Caster.transform.position,
                Caster.Forward * Data.ExplosionRadius, 
                Data.ExplosionRadius, 
                colliders,
                GameMetrics.Global.HittableLayers,
                QueryTriggerInteraction.Collide);
            
            for (int i = 0; i < size; i++)
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