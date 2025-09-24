using System;
using System.Collections.Generic;
using System.Reflection;
using LTX.Singletons.Settings;
using UnityEngine;

namespace LTX.Singletons
{
    public static class SceneSingletonUtility
    {
        private static Dictionary<Type, SceneSingletonMode> sceneSingletonModeCache;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Load()
        {
            sceneSingletonModeCache = new Dictionary<Type, SceneSingletonMode>();
        }


        public static bool IsCrossScene<T>() where T : SceneSingleton<T>
        {
            Type type = typeof(T);
            SceneSingletonMode value = SceneSingletonMode.UseSettings;

            if (!sceneSingletonModeCache.TryGetValue(type, out value))
            {
                var attribute = type.GetCustomAttribute(typeof(OverrideCrossSceneBehaviourAttribute));
                if (attribute is OverrideCrossSceneBehaviourAttribute overrideCrossSceneBehaviourAttribute)
                    value = overrideCrossSceneBehaviourAttribute.sceneSingletonMode;
                else
                    sceneSingletonModeCache.Add(type, SceneSingletonMode.UseSettings);
            }

            return value switch
            {
                SceneSingletonMode.OnePerScene => false,
                SceneSingletonMode.OneAcrossAllScene => true,
                SceneSingletonMode.UseSettings => SingletonSettings.Current.AllowMultiSceneSingletonByDefault,
                _ => false
            };
        }
    }
}