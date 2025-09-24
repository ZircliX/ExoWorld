using System;
using System.Collections.Generic;
using LTX.ChanneledProperties.Settings;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct ChannelKey : IEquatable<ChannelKey>
    {
        internal bool isValid;
        private Guid guid;
        private int hash;
        public Guid Guid => guid;

        [field: SerializeField]
        public Object Source { get; private set; }
        [field: SerializeField]
        public string PointerTag { get; private set; }
        
        private ChannelKey(Guid guid)
        {
            this.guid = guid;
            isValid = true;
            PointerTag = null;
            Source = null;
            hash = guid.GetHashCode();
        }

        public override int GetHashCode() => hash;
        public bool Equals(ChannelKey other) => guid.Equals(other.guid);

        public override bool Equals(object obj) => obj is ChannelKey other && Equals(other);

        #region Static

        internal static readonly ChannelKey None = new ChannelKey(System.Guid.Empty)
        {
            isValid = false
        };

        private static Dictionary<Object, ChannelKey> unityObjectsKeys;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void InitializeOnLoad()
        {
            if (ChanneledPropertiesSettings.Current.LogInitialisationMessages)
                Debug.Log($"[Channeled properties] Initializing channel keys...");

            unityObjectsKeys = new();

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
            ;
        }

        private static void SceneManager_sceneUnloaded(Scene scene) => CleanMissingReferences();

        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode) =>
            CleanMissingReferences();

        private static void CleanMissingReferences()
        {
            if (ChanneledPropertiesSettings.Current.LogInitialisationMessages)
                Debug.Log($"[Channeled properties] Clearing unity channel keys...");

            using (DictionaryPool<Object, ChannelKey>.Get(out Dictionary<Object, ChannelKey> validPairs))
            {
                foreach ((Object key, ChannelKey value) in unityObjectsKeys)
                {
                    if (key != null)
                        validPairs.Add(key, value);
                }

                unityObjectsKeys.Clear();
                foreach ((Object key, ChannelKey channelKey) in validPairs)
                    unityObjectsKeys.Add(key, channelKey);
            }
        }

        public static ChannelKey GetUniqueChannelKey() => new(System.Guid.NewGuid());


        public static ChannelKey GetUniqueChannelKey(string pointer)
        {
            var key = GetUniqueChannelKey();
            key.PointerTag = pointer;

            return key;
        }

        public static ChannelKey GetUniqueChannelKey<T>(T pointer)
        {
            ChannelKey channelKey = GetUniqueChannelKey(typeof(T).Name);

            if (pointer is not Object unityObject)
                return channelKey;

            channelKey.Source = unityObject;
            unityObjectsKeys.Add(unityObject, channelKey);
            return channelKey;
        }

        public static implicit operator ChannelKey(Object unityObject)
        {
            if (unityObjectsKeys == null)
                return default;

            if (unityObjectsKeys.TryGetValue(unityObject, out ChannelKey channelKey))
                return channelKey;
            else
                return GetUniqueChannelKey(unityObject);
        }

        #endregion

    }
}