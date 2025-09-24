using System;

namespace LTX.Singletons
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OverrideCrossSceneBehaviourAttribute : Attribute
    {
        public readonly SceneSingletonMode sceneSingletonMode;
        
        public OverrideCrossSceneBehaviourAttribute(SceneSingletonMode sceneSingletonMode)
        {
            this.sceneSingletonMode = sceneSingletonMode;
        }
    }
}