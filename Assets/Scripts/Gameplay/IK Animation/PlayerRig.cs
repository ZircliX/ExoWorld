using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.IK_Animation
{
    public class PlayerRig : MonoBehaviour
    {
        [SerializeField] private CustomParentConstraint handR;
        [SerializeField] private CustomParentConstraint handL;
        [SerializeField] private Transform headModel;

        private WeaponRig currentWeapon;

        public void OnWeaponChange(WeaponRig weaponRig)
        {
            currentWeapon = weaponRig;
            
            handR.SetTarget(currentWeapon.targetHandR);
            handL.SetTarget(currentWeapon.targetHandL);
        }

        public void EnableHead(bool state)
        {
            headModel.gameObject.SetActive(state);
        }
    }
}