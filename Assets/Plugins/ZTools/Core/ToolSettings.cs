using UnityEngine;
using ZTools.Logger.Core.Enums;

namespace ZTools.Core.Settings
{
    [System.Serializable]
    public struct ToolSettings
    {
        [field : SerializeField] public ToolDefinition ToolType { get; private set; }
    }
}