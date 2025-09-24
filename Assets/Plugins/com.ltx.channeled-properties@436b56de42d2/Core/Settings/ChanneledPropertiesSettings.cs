using LTX.Tools.Settings;
using LTX.Tools.Settings.Attributes;
using UnityEngine;

namespace LTX.ChanneledProperties.Settings
{
    [LTXSettingsTitle("Channeled Properties", "#bfdeb1")]
    public class ChanneledPropertiesSettings : LTXSettingsAsset<ChanneledPropertiesSettings>
    {
        [field: Header("Common")]
        [field: SerializeField]
        public bool LogInitialisationMessages { get; private set; } = true;
        [field: SerializeField]
        public bool ForceCapacityToNextPowerOfTwo { get; private set; }
        [field: SerializeField, Range(2, 32)]
        public int DefaultCapacity { get; private set; } = 16;
        [field: SerializeField]
        public bool ExpandOnFullCapacityReached { get; private set; } = true;

        [field: Header("Formula")]

        [field: SerializeField, Range(1, 128)]
        public int MaxGroups { get; private set; } = 32;
    }
}