using UnityEngine;

namespace ZTools.Logger.Core.Enums
{
    [CreateAssetMenu(fileName = "ToolName", menuName = "ZTools/Logger/ToolDefinition", order = 1)]
    public class ToolDefinition : ScriptableObject
    {
        [field: SerializeField] public string ToolID { get; private set; }
        [field: SerializeField] public LogMode LogMode { get; private set; }
    }
}