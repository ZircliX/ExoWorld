using Helteix.Tools.Settings;
using UnityEngine;

namespace OverBang.Pooling
{
    [AutoGenerateGameSettings]
    [GameSettingsTitle("Pooling System", "#FF00FF")]
    [CreateAssetMenu(fileName = "New PoolingSettings", menuName = "OverBang/Pooling/Settings")]
    public class PoolingSettings : GameSettings<PoolingSettings>
    {
        [field: SerializeField] 
        public int MaxInstancePerFrame { get; private set; }
        
        [field: SerializeField] 
        public bool HidePools { get; private set; }
    }
}