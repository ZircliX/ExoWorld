using OverBang.ExoWorld.Gameplay.Cameras.Composits;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Cameras.Data
{
    [CreateAssetMenu(menuName = "OverBang/Camera/CameraShakeData")]
    public class CameraShakeData : ScriptableObject
    {
        [field: SerializeField] public CameraShakeComposite CameraShakeComposite { get; protected set; }
    }
}