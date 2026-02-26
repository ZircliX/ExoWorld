using System.Collections.Generic;
using Helteix.Tools;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Movement;
using OverBang.ExoWorld.Gameplay.Player;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AppareilZetaEntity : NetworkBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField] private DetectionArea bonusDetectionArea;
        [SerializeField] private DetectionArea damageDetectionArea;
        
        private IAppareilZetaAbilityStrategyData strategyData;
        private IExplosionStrategy explosionStrategy;
        
        private List<PlayerEntity> players;
        
        private List<IDamageable> damageables;
        private DynamicBuffer<IDamageable> buffer;

        private float currentTime;
        private const float TICK = 1;

        public void Initialize(IAppareilZetaAbilityStrategyData strategyData, AppareilZetaData data, Vector3 direction)
        {
            players = new List<PlayerEntity>(4);
            damageables = new List<IDamageable>(8);
            buffer = new DynamicBuffer<IDamageable>(8);
            
            this.strategyData = strategyData;
            explosionStrategy = new StandardExplosion(new DamageData()
            {
                baseDamage = strategyData.ExplosionDamage
            });
            
            bonusDetectionArea.GetCollider<SphereCollider>().radius = strategyData.ExplosionRadius;
            bonusDetectionArea.SetAllowedTags("LocalPlayer");
            bonusDetectionArea.SetRequireInterface<PlayerEntity>();

            bonusDetectionArea.OnEnter += OnEnter;
            bonusDetectionArea.OnExit += OnExit;
            
            damageDetectionArea.GetCollider<SphereCollider>().radius = strategyData.ExplosionRadius;
            bonusDetectionArea.SetRequireInterface<IDamageable>();
            
            damageDetectionArea.OnEnter += OnEnter;
            damageDetectionArea.OnExit += OnExit;
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
            Invoke(nameof(FreezePosition), 1f);
        }
        
        private void FreezePosition()
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            
            explosionStrategy.Explode(() =>
            {
                Collider[] colliders = Physics.OverlapSphere(
                    transform.position,
                    strategyData.ExplosionRadius,
                    GameMetrics.Global.HittableLayers,
                    QueryTriggerInteraction.Collide);

                return colliders;
            });
        }

        public void End()
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                PlayerEntity player = players[i];
                
                player.WeaponController.SetShootRateMultiplier(1);
                player.WeaponController.SetDamageMultiplier(1);

                if (player.TryGetComponent(out PlayerMovement pm))
                {
                    pm.SetMovementSpeedMultiplier(1);
                }
            }
        }

        public void Tick(float deltaTime)
        {
            currentTime += deltaTime;

            if (currentTime >= TICK)
            {
                currentTime = 0;
                
                buffer.CopyFrom(damageables);
                for (int i = 0; i < buffer.Length; i++)
                {
                    IDamageable damageable = buffer[i];
                    
                    RuntimeDamageData runtimeDamageData = new RuntimeDamageData()
                    {
                        finalDamage = strategyData.ExplosionDamageTick
                    };
                    
                    damageable.TakeDamage(runtimeDamageData);
                }
            }
        }

        private void OnEnter(Collider col, object target)
        {
            if (target is PlayerEntity player)
            {
                players.Add(player);
                
                player.WeaponController.SetShootRateMultiplier(1 - 1 * strategyData.GivenBonus);
                player.WeaponController.SetDamageMultiplier(1 + 1 * strategyData.GivenBonus);

                if (player.TryGetComponent(out PlayerMovement pm))
                {
                    pm.SetMovementSpeedMultiplier(1 + 1 * strategyData.GivenBonus);
                }
            }
            
            else if (target is IDamageable damageable)
            {
                damageables.Add(damageable);
            }
        }

        private void OnExit(Collider col, object target)
        {
            if (target is PlayerEntity player)
            {
                players.Remove(player);
                
                player.WeaponController.SetShootRateMultiplier(1);
                player.WeaponController.SetDamageMultiplier(1);

                if (player.TryGetComponent(out PlayerMovement pm))
                {
                    pm.SetMovementSpeedMultiplier(1);
                }
            }
            
            else if (target is IDamageable damageable)
            {
                damageables.Remove(damageable);
            }
        }
    }
}