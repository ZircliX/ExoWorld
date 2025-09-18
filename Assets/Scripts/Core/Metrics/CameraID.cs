using UnityEngine;

namespace OverBang.GameName.Metrics
{
    [CreateAssetMenu(menuName = "OverBang/Metrics/CameraID")]
    public class CameraID : ScriptableObject
    {
        public static CameraID PlayerView => GameMetrics.Global.PlayerView;
        public static CameraID PlayerSpectate => GameMetrics.Global.PlayerView;
        public static CameraID MainMenu => GameMetrics.Global.MainMenu;
    }
}