using System;
using System.Collections.Generic;
using System.Linq;
using OverBang.Pooling.PoolStrategies;
using OverBang.Pooling.Resource;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace OverBang.Pooling
{
    [System.Serializable]
    public class Pool<T> : IPool, IDisposable where T : Object
    {

        private struct PooledElement : IEquatable<PooledElement>
        {
            public T instance;
            public IPoolInstanceListener[] listeners;
            
            public bool Equals(PooledElement other)
            {
                return EqualityComparer<T>.Default.Equals(instance, other.instance) && Equals(listeners, other.listeners);
            }

            public override bool Equals(object obj)
            {
                return obj is PooledElement other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(instance);
            }
        }   
        
        [field: SerializeField]
        public int Size { get; private set; }

        [field: SerializeField]
        public PoolResource PoolResource { get; private set; }

        [field: SerializeField] public T LoadedAsset { get; private set; } 
        public bool IsLoaded { get; private set; }
        
        public int CurrentPoolSize => pooledObjects.Count + releasedObjects.Count;

        private Awaitable fillJob;

        private Stack<PooledElement> pooledObjects;
        private Dictionary<T, PooledElement> releasedObjects;
        private Queue<T> releaseOrder;

        private readonly GameObject root;
        private IPoolStrategy strategy;
        
        public Pool(Transform parent, params IPoolConfig[] configs) : this(parent, configs.AsEnumerable()) { }
        public Pool(Transform parent, IEnumerable<IPoolConfig> configs)
        {
            foreach (IPoolConfig config in configs)
            {
                if (PoolResource == null)
                {
                    PoolResource = config.PoolResource;
                }
                else if (config.PoolResource != PoolResource)
                {
                    continue;
                }
                
                Size += config.PoolSize;
            }

            root = new GameObject($"{PoolResource?.name}_Pool");
            root.transform.SetParent(parent);
            root.hideFlags = (PoolingSettings.Current.HidePools ? HideFlags.HideInInspector : HideFlags.None) | HideFlags.NotEditable;
            
            pooledObjects = new Stack<PooledElement>(Size);
            releasedObjects = new Dictionary<T, PooledElement>(Size);
            releaseOrder = new Queue<T>(Size);
        }

        public void Load()
        {
            if (IsLoaded)
                return;

            PoolResource.Asset.Load(o =>
            {
                if (o is T asset)
                {
                    strategy = PoolStrategyUtilities.GetStrategyFor(asset);
                    
                    IsLoaded = true;
                    LoadedAsset = asset;
                    
                    strategy?.ProcessLoadedAsset(asset);
                    AsyncFillPool();
                }
            });
        }

        private void AsyncFillPool()
        {
            fillJob = CreateAwaitable();
            return;
            
            async Awaitable CreateAwaitable()
            {
                using (ListPool<IPoolInstanceListener>.Get(out List<IPoolInstanceListener> listeners))
                {
                    int missing = Size - CurrentPoolSize;

                    int currentFrameCount = 0;
                    for (int i = 0; i < missing; i++)
                    {
                        if (currentFrameCount >= PoolingSettings.Current.MaxInstancePerFrame)
                        {
                            await Awaitable.NextFrameAsync();
                            currentFrameCount = 0;
                        }

                        CreateNewPooledInstance(listeners);

                        currentFrameCount++;
                    }
                }

                fillJob = null;
            }
        }

        private void SyncFillPool()
        {
            using (ListPool<IPoolInstanceListener>.Get(out List<IPoolInstanceListener> listeners))
            {
                int missing = Size - CurrentPoolSize;

                for (int i = 0; i < missing; i++)
                    CreateNewPooledInstance(listeners);
            }
        }

        private void CreateNewPooledInstance(List<IPoolInstanceListener> listeners)
        {
            T instance = Object.Instantiate(LoadedAsset, root.transform);

            listeners.Clear();
            strategy?.CollectListeners(instance, listeners);
            
            PooledElement pooledElement = new PooledElement()
            {
                instance = instance,
                listeners = listeners.ToArray()
            };

            strategy?.PreparePooledInstance(instance);

            pooledObjects.Push(pooledElement);
        }

        public T GenericSpawn()
        {
            if (pooledObjects.TryPop(out PooledElement element))
            {
                releasedObjects.Add(element.instance, element);
                releaseOrder.Enqueue(element.instance);
                strategy?.OnPreSpawn(element.instance);
                for (int i = 0; i < element.listeners.Length; i++)
                {
                    IPoolInstanceListener listener = element.listeners[i];
                    listener.OnSpawn(this);
                }

                strategy?.OnPostSpawn(element.instance);
            }
            else if (fillJob is null)
            {
                Debug.Log("Pool empty, applying behavior: " + PoolResource.PoolEmptyBehavior);
                switch (PoolResource.PoolEmptyBehavior)
                {
                    case PoolEmptyBehavior.DontSpawn: return null;
                    case PoolEmptyBehavior.ExtendByOne:
                        Size++;
                        SyncFillPool();
                        return GenericSpawn();
                    case PoolEmptyBehavior.ExtendByDouble:
                        Size *= 2;
                        AsyncFillPool();
                        return GenericSpawn();
                    case PoolEmptyBehavior.ExtendByNextPowerOfTwo:
                        Size = Mathf.NextPowerOfTwo(Size + 1);
                        AsyncFillPool();
                        return GenericSpawn();
                    case PoolEmptyBehavior.Loop:
                        while (releaseOrder.Count > 0)
                        {
                            T oldest = releaseOrder.Dequeue();
                            if (releasedObjects.ContainsKey(oldest))
                            {
                                Despawn(oldest);
                                return GenericSpawn();
                            }
                        }

                        return null;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                fillJob?.Cancel();
                SyncFillPool();
                return GenericSpawn();
            }

            return element.instance;
        }

        public Object Spawn()
        {
            T genericSpawn = GenericSpawn();
            return genericSpawn;
        }

        public void Despawn(T instance)
        {
            if (releasedObjects.Remove(instance, out PooledElement element))
            {
                strategy?.OnPreDestroy(element.instance);

                for (int index = 0; index < element.listeners.Length; index++)
                {
                    IPoolInstanceListener listener = element.listeners[index];
                    listener.OnDespawn(this);
                }
                
                strategy?.OnPostDestroy(element.instance);
                
                pooledObjects.Push(element);
            }
        }

        public void Despawn(Object instance)
        {
            if (instance is T typed)
                Despawn(typed);
            else
                Debug.LogWarning($"Tried to despawn object of type {instance.GetType()} in pool of {typeof(T)}");
        }

        public void Dispose()
        {
            using (ListPool<T>.Get(out List<T> buffer))
            {
                buffer.AddRange(releasedObjects.Keys);

                foreach (T instance in buffer)
                {
                    Despawn(instance);
                }
            }

            using (ListPool<PooledElement>.Get(out List<PooledElement> buffer))
            {
                buffer.AddRange(pooledObjects);

                foreach (PooledElement pooledElement in buffer)
                {
                    Object.Destroy(pooledElement.instance);
                }
                
                pooledObjects.Clear();
            }
            
            PoolResource.Asset.Unload();
        }
    }
}