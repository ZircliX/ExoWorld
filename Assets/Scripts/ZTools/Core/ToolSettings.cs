using UnityEngine;

namespace ZTools.Core.ZTools.Core
{
    [System.Serializable]
    public struct ToolSettings
    {
        [field : SerializeField] public ToolDefinition ToolType { get; private set; }
    }
}