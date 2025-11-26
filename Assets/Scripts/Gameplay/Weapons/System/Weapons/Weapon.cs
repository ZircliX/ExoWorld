using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public abstract class Weapon : MonoBehaviour, IPlayerComponent
    {
        [field: SerializeField]
        public WeaponData WeaponData { get; protected set; }
        public RuntimeWeaponState State { get; protected set; }

        protected IFireBehaviour fireBehaviour;
        protected IReloadBehaviour reloadBehaviour;
        public PlayerController Controller { get; set; }
        
        public void OnSync(CharacterData data, Animator animator)
        {
            Initialize(WeaponData);
        }

        public virtual void Initialize(WeaponData weaponData)
        {
            WeaponData = weaponData;
            State = new RuntimeWeaponState(WeaponData);

            fireBehaviour = WeaponData.FireBehaviour.CreateFireBehavior();
            reloadBehaviour = WeaponData.ReloadBehaviour.CreateReloadBehavior();

            fireBehaviour?.OnInitialize(this);
            reloadBehaviour?.OnInitialize(this);

            Instantiate(weaponData.WeaponPrefab, transform);
        }
        
        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            
            State.Tick(deltaTime);
            fireBehaviour?.Tick(deltaTime);
            reloadBehaviour?.Tick(deltaTime);
        }
        
        public abstract void Fire();

        public virtual void OnShootInput(InputAction.CallbackContext context)
        {
            fireBehaviour?.OnShootInput(context);
        }

        public virtual void OnReloadInput(InputAction.CallbackContext context)
        {
            reloadBehaviour?.OnReloadInput(context);
        }
        
        protected Vector3 GetBulletDirection(Transform shootPoint, Vector2 dispersionDeg)
        {
            float angleX = Random.Range(-dispersionDeg.x, dispersionDeg.x) * Mathf.Deg2Rad;
            float angleY = Random.Range(-dispersionDeg.y, dispersionDeg.y) * Mathf.Deg2Rad;

            Vector3 forward = shootPoint.forward;
            Vector3 right = shootPoint.right;
            Vector3 up = shootPoint.up;

            // Rotate in each axis independently
            Vector3 offset =
                right * Mathf.Tan(angleX) +
                up * Mathf.Tan(angleY);

            return (forward + offset).normalized;
        }
    }
}