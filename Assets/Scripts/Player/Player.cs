using Combat.Weapon;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform orientation;
        private void Update()
        {
            if(!IsOwner) return;
            if (Input.GetMouseButtonDown(0))
            {
                weapon.Shoot(orientation.forward);
            }
        }
        
    }
}