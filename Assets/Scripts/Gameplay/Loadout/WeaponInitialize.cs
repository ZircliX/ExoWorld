using KBCore.Refs;
using OverBang.ExoWorld.Core.Utils;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class WeaponInitialize : MonoBehaviour
    {
        [SerializeField, Self] private Weapon weapon;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private Camera cam;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            Awaitable aw = PoolUtils.SetupPooling(null);
            aw.Run();
            weapon.Initialize(weaponData, cam);
        }
    }
}