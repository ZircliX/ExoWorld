using DG.Tweening;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.Crosshair
{
    [CreateAssetMenu(fileName = "CrosshairOffsetData", menuName = "OverBang/CrosshairOffsetData")]
    public class CrosshairOffsetData : ScriptableObject
    {
        [field: SerializeField] public Vector2 MinOffset { get; private set; }
        [field: SerializeField] public float MaxOffsetTime { get; private set; }
        [field: SerializeField] public Ease MaxEase{ get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public Vector2 MaxOffset { get; private set; }
        [field: SerializeField] public float MinOffsetTime { get; private set; }
        [field: SerializeField] public Ease MinEase{ get; private set; }
    }
}