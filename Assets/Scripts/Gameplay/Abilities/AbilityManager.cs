using System;
using System.Collections.Generic;
using System.Reflection;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using UnityEngine;

[assembly:LookForAbilityStrategies]
namespace OverBang.GameName.Gameplay
{
    public static class AbilityManager
    {
        private static readonly Dictionary<Type, Type> strategies = new Dictionary<Type, Type>();
        private static readonly Type abilityStrategyGenericType = typeof(IAbilityStrategy<,>);
        
        static AbilityManager()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                object[] asmAttributes = asm.GetCustomAttributes(typeof(LookForAbilityStrategiesAttribute), false);
                if (asmAttributes.Length == 0) 
                    continue;
                
                Type[] classes = asm.GetTypes();
                foreach (Type type in classes)
                {
                    ScanForStrategies(type);
                }
            }
        }

        private static void ScanForStrategies(Type type)
        {
            IEnumerable<CreateStrategyForAttribute> attributes = type.GetCustomAttributes<CreateStrategyForAttribute>();

            foreach (CreateStrategyForAttribute strategy in attributes)
            { 
                strategies[strategy.strategyDataType] = type;
                return;
            }
        }

        public static IAbilityStrategy<T> CreateStrategyFor<T>(this IAbilityStrategyData strategyData)
            where T : AbilityData
        {
            Type strategyDataType = strategyData.GetType();
            Type strategyGenericType = abilityStrategyGenericType.MakeGenericType(typeof(T), strategyDataType);
            
            if (strategies.TryGetValue(strategyDataType, out Type strategyType))
            {
                if (strategyGenericType.IsAssignableFrom(strategyType))
                {
                    return (IAbilityStrategy<T>)Activator.CreateInstance(strategyType);
                }
            }
            
            Debug.LogError($"No strategy found for strategy data type: {strategyDataType.Name}");
            return null;
        }

        public static IAbility CreateAbilityFor<TData>(this TData data, IAbilityCaster caster)
            where TData : AbilityData
        {
            Type abilityDataType = data.GetType();
            Type cooldownAbilityType = 
                typeof(CooldownAbility<>).
                MakeGenericType(abilityDataType);
            
            object ability = Activator.CreateInstance(cooldownAbilityType, data, caster);
            return ability as IAbility;
        }
    }
}