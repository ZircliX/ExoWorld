using System;

namespace OverBang.ExoWorld.Core.GameMode
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