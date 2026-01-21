using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    [CreateAssetMenu(menuName = "OverBang/Metrics/CameraID")]
    public class CameraID : ScriptableObject
    {
        public static CameraID PlayerViewCamera => CameraIDs.Global.PlayerViewCamera;
        public static CameraID PlayerSpectateCamera => CameraIDs.Global.PlayerSpectateCamera;
        public static CameraID DefaultCamera => CameraIDs.Global.DefaultCamera;
        public static CameraID LoadoutCamera => CameraIDs.Global.LoadoutCamera;
        public static CameraID UpgradeCamera => CameraIDs.Global.UpgradeCamera;
    }
}