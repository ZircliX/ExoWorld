using System;
using UnityEngine;

namespace LTX.Singletons
{
    [DefaultExecutionOrder(-20)]
    public abstract class MonoSingleton<T, TFactory, TFinder> : MonoBehaviour, ISingleton
        where T : MonoSingleton<T, TFactory, TFinder>, ISingleton
        where TFactory : ISingletonFactory<T>, new()
        where TFinder : ISingletonFinder<T>, new()
    {

        public bool IsInstance => HasInstance && Holder.IsInstance;
        public static bool HasInstance => Holder.InternalHasInstance;
        public static T Instance => Holder.InternalInstance;

        private static MonoSingletonHolder<T, TFactory, TFinder> holder;
        private static MonoSingletonHolder<T, TFactory, TFinder> Holder => holder??=new();

        internal MonoSingletonHolder<T, TFactory, TFinder> InstanceHolder => Holder;

        protected virtual void Awake()
        {
            if (Instance != this)
            {
                Holder.Dispose();
                OnExistingInstanceFound(Instance);
            }

            DontDestroyOnLoad(gameObject);
        }


        private void Reset()
        {
            if (!Application.isPlaying)
            {
                Destroy(this);
                Debug.LogError("MonoSingleton cannot be added in editor. They have to be dynamically created on the first access");
            }
        }

        protected virtual void OnExistingInstanceFound(T existingInstance)
        {

        }

        protected virtual void OnDestroy()
        {
            if (IsInstance)
            {
                holder?.Dispose();
                holder = null;
            }
        }
    }
    [DefaultExecutionOrder(-20)]
    public abstract class MonoSingleton<T, TFactory> : MonoSingleton<T,TFactory, MonoSingletonFinder<T>>
        where T : MonoSingleton<T,TFactory, MonoSingletonFinder<T>>, ISingleton
        where TFactory : ISingletonFactory<T>, new()
    {

    }
    [DefaultExecutionOrder(-20)]
    public abstract class MonoSingleton<T> : MonoSingleton<T,MonoSingletonFactory<T>, MonoSingletonFinder<T>>, ISingleton
        where T :MonoSingleton<T,MonoSingletonFactory<T>, MonoSingletonFinder<T>>, ISingleton
    {

    }
}