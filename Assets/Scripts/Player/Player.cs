using Combat.Weapon;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform orientation;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                weapon.Shoot(orientation.forward);
            }
        }
        
    }
}