using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Core
{
    public static class PhaseManager
    {
        private static readonly HashSet<IPhaseListener> listeners = new HashSet<IPhaseListener>();
        private static readonly Dictionary<Type, IPhase> currentPhases = new Dictionary<Type, IPhase>();

        public static bool Register<T>(this IPhaseListener<T> listener) where T : IPhase
        {
            bool added = listeners.Add(listener);
            if (currentPhases.TryGetValue(typeof(T), out IPhase phaseObj) && phaseObj is T phase)
            {
                listener.OnBegin(phase);
            }

            return added;
        }

        public static bool Unregister<T>(this IPhaseListener<T> listener) where T : IPhase
        {
            return listeners.Remove(listener);
        }

        public static Awaitable Run<T>(this T phase) where T : IPhase
            => RunAsync(phase);
        
        public static async Awaitable RunAsync<T>(this T phase)  where T : IPhase
        {
            using (ListPool<IPhaseListener<T>>.Get(out List<IPhaseListener<T>> compatibles))
            {
                currentPhases[typeof(T)] = phase;
                
                //Debug.Log("Beginning phase: " + typeof(T).Name);
                await phase.OnBegin();
                await Awaitable.EndOfFrameAsync();

                foreach (IPhaseListener phaseListener in listeners)
                    if (phaseListener is IPhaseListener<T> compatible)
                        compatibles.Add(compatible);
                
                //Debug.Log("Calling listeners for phase begin: " + typeof(T).Name);
                foreach (IPhaseListener<T> phaseListener in compatibles)
                    phaseListener.OnBegin(phase);
                
                //Debug.Log("Executing phase: " + typeof(T).Name);
                await phase.Execute();
                
                compatibles.Clear();
                foreach (IPhaseListener phaseListener in listeners)
                    if (phaseListener is IPhaseListener<T> compatible)
                        compatibles.Add(compatible);
                
                //Debug.Log("Calling listeners for phase end: " + typeof(T).Name);
                foreach (IPhaseListener<T> phaseListener in compatibles)
                    phaseListener.OnEnd(phase);
                
                await Awaitable.EndOfFrameAsync();
                //Debug.Log("Ending phase: " + typeof(T).Name);
                await phase.OnEnd();
                
                currentPhases.Remove(typeof(T));
            }
        }
    }
}