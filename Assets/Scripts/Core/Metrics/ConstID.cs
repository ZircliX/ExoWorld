using UnityEngine;

namespace OverBang.ExoWorld.Core.Metrics
{
    [System.Serializable]
    public struct ConstID
    {
        public static ConstID Global => GameMetrics.Global.ConstID;
        
        [field: SerializeField] public string DiscordLink { get; private set; }
        [field: SerializeField] public string InstagramLink { get; private set; }
        
        [field: SerializeField] public string PlayerPropertyPlayerName { get; private set; }
        [field: SerializeField] public string PlayerPropertyCharacterData { get; private set; }
        [field: SerializeField] public string PlayerPropertyClientID { get; private set; }
        [field: SerializeField] public string PlayerPropertyHost { get; private set; }
        [field: SerializeField] public string PlayerPropertyHealth { get; private set; }
        [field: SerializeField] public string PlayerPropertyMaxHealth { get; private set; }
        [field: SerializeField] public string PlayerPropertyState { get; private set; }
        [field: SerializeField] public string PlayerPropertyPhaseStatus { get; private set; }
    }
}