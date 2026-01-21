using OverBang.ExoWorld.Gameplay.Composits;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Data
{
    [CreateAssetMenu(menuName = "OverBang/Camera/CameraEffectData")]
    public class CameraEffectData : ScriptableObject
    {
        [field: Header("Parameters")]
        [field: SerializeField] public CameraEffectComposite CameraEffectComposite { get; protected set; }
    }
}