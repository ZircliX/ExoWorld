using UnityEngine;

namespace OverBang.GameName.Core.Metrics
{
    [System.Serializable]
    public struct ConstID
    {
        public static ConstID Global => GameMetrics.Global.ConstID;
        
        [field: SerializeField] public string PlayerPropertyCharacterData { get; private set; }
    }
}