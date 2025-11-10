using UnityEngine;
using ZTools.Core.ZTools.Core.Enums;

namespace ZTools.Core.ZTools.Core
{
    [System.Serializable]
    public struct ToolSettings
    {
        [field : SerializeField] public ToolDefinition ToolType { get; private set; }
    }
}