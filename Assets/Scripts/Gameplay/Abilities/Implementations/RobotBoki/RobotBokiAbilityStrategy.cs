using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using Unity.Netcode;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class RobotBokiAbilityStrategy<TData> : IAbilityStrategy<RobotBokiData, TData>
        where TData : IRobotBokiAbilityStrategyData, IAbilityStrategyData
    {
        protected RobotBoki Robot { get; private set; }
        
        protected TData Data { get; private set; }
        protected ICaster Caster { get; private set; }
        private IAbility<RobotBokiData> ability;
        
        private NetworkSpawnManager spawnManager;
        private LocalGamePlayer localPlayer;
        
        public void Initialize(IAbility<RobotBokiData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            this.ability = ability;
            
            spawnManager = NetworkManager.Singleton.SpawnManager;
            localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
        }
        
        public virtual void Begin(IAbility<RobotBokiData> ability)
        {
            IRobotBehaviour behaviour = GetRobotBehaviour();
            
            NetworkObject networkObject = spawnManager.InstantiateAndSpawn(ability.DataT.Prefab, 
                localPlayer.ClientID,
                true,
                false,
                false,
                Caster.transform.position + Caster.Forward * 1.5f, Caster.transform.rotation);

            if (networkObject.TryGetComponent(out RobotBoki robot))
            {
                Robot = robot;
                
                Robot.Initialize(ability.DataT, behaviour);
                Robot.OnExploded += OnRobotExploded;
            }
        }

        public virtual void Tick(IAbility<RobotBokiData> ability, float deltaTime)
        {
            if (Robot == null)
                return;
            
            Robot.Tick(deltaTime);
        }

        public virtual void End(IAbility<RobotBokiData> ability)
        {
        }

        public void Dispose(IAbility<RobotBokiData> ability)
        {
        }
        
        private void OnRobotExploded()
        {
            ability.End();
        }
        
        protected abstract IRobotBehaviour GetRobotBehaviour();
    }

    [CreateStrategyFor(typeof(RobotBokiStrategyData))]
    public class RobotBokiAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiStrategyData>
    {
        protected override IRobotBehaviour GetRobotBehaviour()
        {
            IRobotBehaviour behaviour = new FollowTargetBehaviour(Data, new StandardExplosion(Data.Damage));
            return behaviour;
        }
    }
    
    [CreateStrategyFor(typeof(RobotBokiLeurreStrategyData))]
    public class RobotBokiLeurreAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiLeurreStrategyData>
    {
        protected override IRobotBehaviour GetRobotBehaviour()
        {
            IRobotBehaviour behaviour = new ForwardBehaviour(Data, new EmptyExplosion());
            return behaviour;
        }
    }
    
    [CreateStrategyFor(typeof(RobotBokiImpulsionStrategyData))]
    public class RobotBokiImpulsionAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiImpulsionStrategyData>
    {
        protected override IRobotBehaviour GetRobotBehaviour()
        {
            IRobotBehaviour behaviour = new FollowTargetBehaviour(Data, new CryoExplosion(Data.Damage, Data.FreezeDuration, 1));
            return behaviour;
        }
    }
}