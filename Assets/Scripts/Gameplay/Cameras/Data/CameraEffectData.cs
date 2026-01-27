using OverBang.ExoWorld.Gameplay.Cameras.Composits;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Cameras.Data
{
    [CreateAssetMenu(menuName = "OverBang/Camera/CameraEffectData")]
    public class CameraEffectData : ScriptableObject
    {
        [field: Header("Parameters")]
        [field: SerializeField] public CameraEffectComposite CameraEffectComposite { get; protected set; }
    }
}