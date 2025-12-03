using UnityEngine;

namespace OverBang.GameName.Core
{
    [System.Serializable]
    public struct CameraIDs
    {
        public static CameraIDs Global => GameMetrics.Global.CameraIDs;
        
        [field: SerializeField] public CameraID PlayerViewCamera { get; private set; }
        [field: SerializeField] public CameraID PlayerSpectateCamera { get; private set; }
        [field: SerializeField] public CameraID DefaultCamera { get; private set; }
        [field: SerializeField] public CameraID LoadoutCamera { get; private set; }
    }
}