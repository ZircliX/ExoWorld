using UnityEngine;

namespace OverBang.GameName.Gameplay.Cameras
{
    [CreateAssetMenu(menuName = "OverBang/Camera/CameraEffectData")]
    public class CameraEffectData : ScriptableObject
    {
        [field: Header("Parameters")]
        [field: SerializeField] public CameraEffectComposite CameraEffectComposite { get; protected set; }
    }
}