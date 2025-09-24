using UnityEngine;

namespace LTX.Tools.Settings
{
    public abstract class LTXSettingsAsset<T> : ScriptableObject where T : ScriptableObject
    {
        public static T Current
        {
            get
            {
                if (asset != null)
                    return asset.GetAsset();

                if (SettingsCollection.Current == null)
                {
                    Debug.LogError("Please access settings at least after assembly loading. Settings are not loaded before this stage.");
                    return null;
                }

                if (SettingsCollection.Current.TryGetSettings(out asset))
                    return asset.GetAsset();

                Debug.LogError($"Couldn't load settings of type {typeof(T).Name}");
                return null;
            }
        }

        private static LTXSettingsAsset<T> asset;

        protected virtual T GetAsset() => this as T;
    }
}