using UnityEngine;

namespace OverBang.GameName.Core
{
    public static class PhaseUtils
    {
        private class PhaseRunner<T> : IPhaseListener<T> where T : IPhase
        {
            public bool IsDone;
            public bool Success;
            
            public void OnBegin(T phase)
            {
                IsDone = false;
            }

            public void OnEnd(T phase, bool success)
            {
                Success = success;
                IsDone = true;
            }
        }

        public static void Register<T>(this IPhaseListener<T> listener) where T : IPhase
        {
            PhaseManager.Global.RegisterListener(listener);
        }

        public static void Unregister<T>(this IPhaseListener<T> listener) where T : IPhase
        {
            PhaseManager.Global.UnregisterListener(listener);
        }
        
        public static async Awaitable Begin<T>(this T phase) where T : IPhase
        {
            await phase.OnBegin();
            PhaseManager.Global.OnBeginPhase(phase);
        }

        public static async Awaitable End<T>(this T phase, bool success) where T : IPhase
        {
            await phase.OnEnd(success);
            PhaseManager.Global.OnEndPhase(phase, success);
        }

        public static async Awaitable<bool> Run<T>(this T phase)  where T : IPhase
        {
            PhaseRunner<T> runner = new PhaseRunner<T>();
            runner.Register();
            await phase.Begin();

            while (!runner.IsDone)
            {
                await Awaitable.NextFrameAsync();
            }
            
            runner.Unregister();
            return runner.Success;
        }
    }
}