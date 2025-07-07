using UnityEngine;

namespace DeadLink.Cameras.Data
{
    [CreateAssetMenu(menuName = "RogueLike/Camera/FallCameraEffectData")]
    public class FallCameraEffectData : CameraEffectData
    {
        [field: Header("Extra Parameters")]
        [field: SerializeField] public float FovMultiplier { get; private set; }
        [field: SerializeField] public float MaxFov { get; private set; }
    }
}