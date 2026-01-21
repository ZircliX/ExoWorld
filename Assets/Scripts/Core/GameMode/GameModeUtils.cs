using System;
using System.Collections.Generic;
using System.Reflection;

namespace OverBang.ExoWorld.Core
{
    public static class GameModeUtils
    {
        private static readonly Dictionary<string, Type> gameModes = new Dictionary<string, Type>();
        
        public const string SurvivalGameModeName = "SurvivalGameMode";
        
        static GameModeUtils()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] classes = asm.GetTypes();
                for (int index = 0; index < classes.Length; index++)
                {
                    Type type = classes[index];
                    object[] attributes = type.GetCustomAttributes(typeof(CreateGameModeAttribute), false);
                    if (attributes.Length == 0) 
                        continue;
                    
                    CreateGameModeAttribute attribute = (CreateGameModeAttribute) attributes[0];
                    
                    gameModes.Add(attribute.gameModeName, type);
                }
            }
        }

        public static bool TryGetGameModeForName(string gameModeName, out Type gameModeType)
        {
            return gameModes.TryGetValue(gameModeName, out gameModeType);
        }
    }
}