using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class RobotBokiAbilityStrategy<TData> : IAbilityStrategy<RobotBokiData, TData>
        where TData : IRobotBokiAbilityStrategyData, IAbilityStrategyData
    {
        protected RobotBoki Robot { get; private set; }
        
        protected TData Data { get; private set; }
        protected ICaster Caster { get; private set; }
        private IAbility<RobotBokiData> ability;
        
        public void Initialize(IAbility<RobotBokiData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            this.ability = ability;
        }
        
        public virtual void Begin(IAbility<RobotBokiData> ability)
        {
            IRobotBehaviour behaviour = GetRobotBehaviour();
            
            Robot = Object.Instantiate(ability.Data.Prefab, Caster.transform.position + Caster.Forward * 1.5f, Caster.transform.rotation);
            Robot.Initialize(ability.Data, behaviour);

            Robot.OnExploded += OnRobotExploded;
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
            Object.Destroy(Robot.gameObject);
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