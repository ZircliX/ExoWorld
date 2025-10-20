using UnityEngine;

namespace OverBang.GameName.Gameplay.Combat
{
    public abstract class Weapon : MonoBehaviour
    {
        public BulletData BulletData { get; protected set; }
        
        public abstract void Initialize(BulletData bulletData);
        
        public abstract void Fire();
        public abstract void Reload();
    }
}