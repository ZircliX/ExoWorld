using UnityEngine;

namespace DeadLink.Cameras.Data
{
    [CreateAssetMenu(menuName = "RogueLike/Camera/CameraEffectData")]
    public class CameraEffectData : ScriptableObject
    {
        [field: Header("Parameters")]
        [field: SerializeField] public CameraEffectComposite CameraEffectComposite { get; protected set; }
    }
}