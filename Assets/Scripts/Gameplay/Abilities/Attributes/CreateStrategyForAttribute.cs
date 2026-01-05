using System;

namespace OverBang.GameName.Gameplay
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