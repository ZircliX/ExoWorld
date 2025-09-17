using UnityEngine;

namespace OverBang.GameName.Gameplay.Cameras
{
    [CreateAssetMenu(menuName = "OverBang/Camera/FallCameraEffectData")]
    public class FallCameraEffectData : CameraEffectData
    {
        [field: Header("Extra Parameters")]
        [field: SerializeField] public float FovMultiplier { get; private set; }
        [field: SerializeField] public float MaxFov { get; private set; }
    }
}