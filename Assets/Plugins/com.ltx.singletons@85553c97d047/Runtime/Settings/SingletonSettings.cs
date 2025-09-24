using LTX.Tools.Settings;
using LTX.Tools.Settings.Attributes;
using UnityEngine;

namespace LTX.Singletons.Settings
{
    [LTXSettingsTitle("Singletons", "#8d7eff")]
    public class SingletonSettings : LTXSettingsAsset<SingletonSettings>
    {
        [field: Header("SceneSingletons")]
        [field: SerializeField]
        public bool AllowMultiSceneSingletonByDefault { get; private set; } = true;
    }
}