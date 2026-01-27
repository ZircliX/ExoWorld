using System;
using System.Threading;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Utils
{
    public static class AwaitableUtils
    {
        static readonly AwaitableCompletionSource completionSource = new();
        
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