using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class NetworkWeaponController : NetworkBehaviour
    {
        [SerializeField] private WeaponController weaponController;
    }
}