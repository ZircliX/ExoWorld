using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Core.GameAssets
{
    public class GameDatabase : IDisposable
    {
        public static GameDatabase Global => GameController.GameDatabase;
        public DatabaseCatalog ActiveCatalog { get; private set; } = new DatabaseCatalog()
        {
            name = "None",
            assetsKeys = new(),
            labels = new(),
        };
        
        private Dictionary<object, Object> loadedAssets = new();
        
        private List<object> runningAsyncOperations = new();
        
        
        public async Awaitable ChangeCatalog(DatabaseCatalog newCatalog)
        {
            while (runningAsyncOperations.Count > 0)
            {
                Debug.Log(runningAsyncOperations.Count);
                await Awaitable.NextFrameAsync();
            }

            // Determine which assets to unload and which to load
            using(ListPool<object>.Get(out List<object> keysToRelease))
                using(ListPool<object>.Get(out List<object> keysToLoad))
                {
                    keysToRelease.AddRange(ActiveCatalog.assetsKeys);

                    if (newCatalog.assetsKeys != null)
                    {
                        // Check for assets that are already loaded and don't need to be released or loaded again
                        foreach (object key in newCatalog.assetsKeys)
                        {
                            if (ActiveCatalog.assetsKeys.Contains(key))
                            {
                                keysToRelease.Remove(key);
                                continue;
                            }
                            
                            keysToLoad.Add(key);
                        }
                    }

                    // Release assets that are no longer needed
                    foreach (object key in keysToRelease)
                    {
                        Unload(key);
                    }
                    
                    //Load new assets
                    foreach (object key in keysToLoad)
                        Load(key);
                }

            if (newCatalog.labels != null)
            {
                foreach (string label in newCatalog.labels)
                    LoadMultiple(label);
            }
            
            ActiveCatalog = newCatalog;
            Debug.Log( $"GameDatabase: Changed catalog to {newCatalog.name}, loaded {loadedAssets.Count} assets.");
        }


        public void Unload(params object[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (loadedAssets.Remove(keys[i], out Object asset))
                    Addressables.Release(asset);
            }
        }

        public AsyncOperationHandle LoadMultiple(object key, Action<Object> onAssetLoaded = null)
        {
            if(runningAsyncOperations.Contains(key))
                return default;
            
            AsyncOperationHandle operation = Addressables.LoadAssetsAsync<Object>(key, result =>
            {
                if (result != null)
                {
                    loadedAssets[key] = result;
                    onAssetLoaded?.Invoke(result);
                }
            });
            
            operation.Completed += _ =>
            {
                runningAsyncOperations.Remove(key);
            };

            runningAsyncOperations.Add(key);
            return operation;
        }
        public AsyncOperationHandle<Object> Load(object key)
        {
            if(loadedAssets.ContainsKey(key))
                return default;
            
            if(runningAsyncOperations.Contains(key))
                return default;
            
            AsyncOperationHandle<Object> operation = Addressables.LoadAssetAsync<Object>(key);
            
            runningAsyncOperations.Add(key);
            operation.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    loadedAssets[key] = handle.Result;
                
                runningAsyncOperations.Remove(key);
            };

            return operation;
        }
        
        public T GetFirstOfType<T>() where T : Object
        {
            foreach (Object asset in loadedAssets.Values)
            {
                if (asset is T typedAsset)
                    return typedAsset;
            }

            return null;
        }
        
        public IEnumerable<T> GetAllOfType<T>() where T : Object
        {
            foreach (Object asset in loadedAssets.Values)
            {
                if (asset is T typedAsset)
                    yield return typedAsset;
            }
        }

        void IDisposable.Dispose()
        {
            foreach ((object key, Object value) in loadedAssets)
                Unload(key);
            loadedAssets.Clear();
            
            foreach(object key in runningAsyncOperations)
                Unload(key);
            runningAsyncOperations.Clear();
        }
    }
}