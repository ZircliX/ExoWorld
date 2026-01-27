using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.IK_Animation
{
    public class PlayerRig : MonoBehaviour
    {
        [SerializeField] private CustomParentConstraint handR;
        [SerializeField] private CustomParentConstraint handL;

        private WeaponRig currentWeapon;

        public void OnWeaponChange(WeaponRig weaponRig)
        {
            currentWeapon = weaponRig;
            
            handR.SetTarget(currentWeapon.targetHandR);
            handL.SetTarget(currentWeapon.targetHandL);
        }
    }
}