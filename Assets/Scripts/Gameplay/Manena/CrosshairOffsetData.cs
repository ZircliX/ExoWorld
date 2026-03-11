using DG.Tweening;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Manena
{
    [CreateAssetMenu(fileName = "CrosshairOffsetData", menuName = "OverBang/CrosshairOffsetData")]
    public class CrosshairOffsetData : ScriptableObject
    {
        [field: SerializeField] public Vector2 MinOffset { get; private set; }
        [field: SerializeField] public Vector2 MaxOffset { get; private set; }
        [field: SerializeField] public float OffsetTime { get; private set; }
        [field: SerializeField] public Ease EaseCrosshair{ get; private set; }
    }
}