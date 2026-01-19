using UnityEngine;

namespace OverBang.GameName.Core
{
    [System.Serializable]
    public struct ConstID
    {
        public static ConstID Global => GameMetrics.Global.ConstID;
        
        [field: SerializeField] public string PlayerPropertyPlayerName { get; private set; }
        [field: SerializeField] public string PlayerPropertyCharacterData { get; private set; }
        [field: SerializeField] public string PlayerPropertyPhaseStatus { get; private set; }
    }
}