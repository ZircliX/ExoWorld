using OverBang.ExoWorld.Core;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class RobotBokiAbilityStrategy<TData> : IAbilityStrategy<RobotBokiData, TData>
        where TData : IRobotBokiAbilityStrategyData, IAbilityStrategyData
    {
        public void Initialize(IAbility<RobotBokiData> ability, ICaster caster, TData data)
        {
        }
        
        public virtual void Begin(IAbility<RobotBokiData> ability)
        {
        }

        public virtual void Tick(IAbility<RobotBokiData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<RobotBokiData> ability)
        {
        }

        public void Dispose(IAbility<RobotBokiData> ability)
        {
        }
    }

    [CreateStrategyFor(typeof(RobotBokiStrategyData))]
    public class RobotBokiAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(RobotBokiLeurreStrategyData))]
    public class RobotBokiLeurreAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiLeurreStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(RobotBokiImpulsionStrategyData))]
    public class RobotBokiImpulsionAbilityStrategy : RobotBokiAbilityStrategy<RobotBokiImpulsionStrategyData>
    {
        
    }
}