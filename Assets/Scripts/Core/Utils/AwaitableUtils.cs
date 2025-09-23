using System;
using System.Threading;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public static class AwaitableUtils
    {
        public static void Run<T>(this Awaitable<T> task)
        {
            _ = RunInternal(task);
        }

        private static async Awaitable RunInternal<T>(Awaitable<T> task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogError($"AsyncRunner exception: {e}");
            }
        }
        
        public static void Run(this Awaitable task)
        {
            _ = RunInternal(task);
        }

        private static async Awaitable RunInternal(Awaitable task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public static async Awaitable AwaitableUntil(Func<bool> condition, CancellationToken cancellationToken)
        {
            while(!condition()){
                cancellationToken.ThrowIfCancellationRequested();
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }
}