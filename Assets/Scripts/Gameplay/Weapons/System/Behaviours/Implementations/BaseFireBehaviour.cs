using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public abstract class BaseFireBehaviour : IFireBehaviour
    {
        public int ConsecutiveShots { get; protected set; }
        public Weapon Weapon { get; protected set; }
        
        protected float consecutiveShotsValue;

        public virtual void OnInitialize(Weapon weapon)
        {
            Weapon = weapon;
        }

        public abstract void OnShootInput(InputAction.CallbackContext context);

        public abstract void Tick(float deltaTime);

        protected virtual void HandleFire()
        {
            if (Weapon == null)
            {
                Debug.LogError($"Weapon is null");
                return;
            }
            
            WeaponData data = Weapon.WeaponData;
            RuntimeWeaponState state = Weapon.State;

            int bulletsToFire = data.BulletsPerShot;
            if (!state.TryConsume(ref bulletsToFire))
            {
                //Debug.LogWarning($"Could not consume {bulletsToFire} bullet(s).");
                return;
            }
            
            // Fire all bullets
            for (int i = 0; i < bulletsToFire; i++)
            {
                Weapon.Fire();
            }
            Weapon.RequestOnWeaponFired();
            // TODO : SOUND
            
            // Increment Consecutive Shots
            if (ConsecutiveShots < Weapon.WeaponData.MaxRecoilShots)
            {
                consecutiveShotsValue++;
                ConsecutiveShots = Mathf.FloorToInt(consecutiveShotsValue);
            }
        }

        // Lerp ConsecutiveShots to 0
        protected virtual void HandleConsecutiveShots()
        {
            if (Weapon == null)
            {
                Debug.LogError($"Weapon is null");
                return;
            }
            
            float resetSpeed = Weapon.WeaponData.ResetLerpSpeed;
            consecutiveShotsValue = Mathf.MoveTowards(
                consecutiveShotsValue,
                0f,
                resetSpeed * Time.deltaTime
            );

            ConsecutiveShots = Mathf.FloorToInt(consecutiveShotsValue);
        }
    }
}