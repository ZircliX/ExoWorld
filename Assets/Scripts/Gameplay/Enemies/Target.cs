using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class Target : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public Transform DamageTarget { get; private set; }
        public void TakeDamage(RuntimeDamageData damage)
        {
            Debug.Log("aajhhfgbsjhgbwsfkygswdkf");
        }
    }
}