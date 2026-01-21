using System;

namespace OverBang.ExoWorld.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateGameModeAttribute : Attribute
    {
        public readonly string gameModeName;
        
        public CreateGameModeAttribute(string gameModeName)
        {
            this.gameModeName = gameModeName;
        } 
    }
}