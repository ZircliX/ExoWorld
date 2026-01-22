using System;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateStrategyForAttribute : System.Attribute
    {
        public readonly Type strategyDataType;
        
        public CreateStrategyForAttribute(Type strategyDataType)
        {
            this.strategyDataType = strategyDataType;
        }
    }
}