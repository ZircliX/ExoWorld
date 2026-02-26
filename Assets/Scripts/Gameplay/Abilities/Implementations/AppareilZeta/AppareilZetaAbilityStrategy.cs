using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Movement;
using OverBang.ExoWorld.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class AppareilZetaAbilityStrategy<TData> : IAbilityStrategy<AppareilZetaData, TData>
        where TData : IAppareilZetaAbilityStrategyData, IAbilityStrategyData
    {
        private TData data;
        private ICaster caster;

        private PlayerEntity playerEntity;
        private PlayerMovement pm;
        
        private NetworkSpawnManager spawnManager;
        private LocalGamePlayer localPlayer;

        private NetworkObject entityNetworkObject;
        private AppareilZetaEntity entity;
        
        public void Initialize(IAbility<AppareilZetaData> ability, ICaster caster, TData data)
        {
            this.data = data;
            this.caster = caster;
            
            spawnManager = NetworkManager.Singleton.SpawnManager;
            localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
        }
        
        public virtual void Begin(IAbility<AppareilZetaData> ability)
        {
            entityNetworkObject = spawnManager.InstantiateAndSpawn(
                ability.DataT.Prefab, 
                localPlayer.ClientID, 
                true, 
                false, 
                false, 
                caster.CastAnchor.position + caster.Forward, Quaternion.identity);

            if (entityNetworkObject.TryGetComponent(out entity))
            {
                entity.Initialize(data, ability.DataT, caster.Forward);
            }
        }

        public virtual void Tick(IAbility<AppareilZetaData> ability, float deltaTime)
        {
            entity?.Tick(deltaTime);
        }

        public virtual void End(IAbility<AppareilZetaData> ability)
        {
            entity?.End();
            
            entityNetworkObject.Despawn(true);

            entityNetworkObject = null;
            entity = null;
        }

        public void Dispose(IAbility<AppareilZetaData> ability)
        {
        }
    }
    
    [CreateStrategyFor(typeof(AppareilZetaStrategyData))]
    public class AppareilZetaAbilityStrategy : AppareilZetaAbilityStrategy<AppareilZetaStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(AppareilZetaPersistantStrategyData))]
    public class AppareilZetaPersistantAbilityStrategy: AppareilZetaAbilityStrategy<AppareilZetaPersistantStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(AppareilZetaCompressedStrategyData))]
    public class AppareilZetaCompressedAbilityStrategy : AppareilZetaAbilityStrategy<AppareilZetaCompressedStrategyData>
    {
        
    }
}