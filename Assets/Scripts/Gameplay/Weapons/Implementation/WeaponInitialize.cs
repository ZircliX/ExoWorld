using KBCore.Refs;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class WeaponInitialize : MonoBehaviour
    {
        [SerializeField, Self] private Weapon weapon;
        [SerializeField] private WeaponData weaponData;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            Awaitable aw = PoolUtils.SetupPooling(null);
            aw.Run();
            weapon.Initialize(weaponData, Camera.main);
        }
    }
}