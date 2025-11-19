using UnityEngine;

namespace OverBang.GameName.Gameplay.Bullets
{
    public abstract class BulletData : ScriptableObject
    {
        [field: SerializeField] public BulletType BulletType { get; protected set; }
    }
}