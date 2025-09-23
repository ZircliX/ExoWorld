using System.Collections.Generic;
using UnityEngine;
using ZTools.Logger.Core.Enums;

namespace ZTools.Core.Settings
{
    [CreateAssetMenu(fileName = "ZToolsSettingsData", menuName = "ZTools/Settings/ZToolsSettingsData", order = 1)]
    public class ZToolsSettingsData : ScriptableObject
    {
        [field: SerializeField] public List<ToolDefinition> ToolDefinitions { get; private set; }
        public static IReadOnlyList<ToolDefinition> GetToolsSettings()
        {
            ZToolsSettingsData settings = Resources.Load<ZToolsSettingsData>("ZToolsSettingsData");
            return settings.ToolDefinitions;
        }
    }
}