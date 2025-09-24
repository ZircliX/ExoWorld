using System;
using UnityEngine;

namespace LTX.Singletons
{
    public class BaseSingleton<T, TFactory, TFinder> : IDisposable, ISingleton
        where T : class, ISingleton
        where TFactory : ISingletonFactory<T>, new()
        where TFinder : ISingletonFinder<T>, new()
    {

        private static TFactory _factory;
        protected static TFactory Factory => _factory??=new();

        private static TFinder _finder;
        protected static TFinder Finder => _finder??=new();

        protected static T instance;

        public static bool HasInstance => instance != null || TryFindInstance();
        public virtual bool IsInstance => HasInstance && instance.Equals(this);


        private static bool TryFindInstance()
        {
            if (Finder.TryFindExistingInstance(out var foundInstance))
            {
                Instance = foundInstance;
                return true;
            }

            Instance = null;
            return false;
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static T Instance
        {
            get
            {
                if (HasInstance)
                    return instance;

                if (!Application.isPlaying)
                {
                    Debug.LogWarning($"[Singleton] Instance {typeof(T)} cannot be created in editor mode.");
                    return null;
                }

                try
                {
                    //If none was found, a new fresh instance is created with the factory
                    instance = Factory.CreateSingleton();
                    return instance;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }

            private set => instance = value;
        }


        public virtual void Dispose()
        {
            if (IsInstance)
            {
                Instance = null;
            }
        }
    }
}