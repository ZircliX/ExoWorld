using System;
using KBCore.Refs;
using OverBang.ExoWorld.Gameplay.IK_Animation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData WeaponData { get; protected set; }
        public RuntimeWeaponState State { get; protected set; }
        [field: SerializeField, Self] public WeaponRig Rig { get; private set; }
        [field: SerializeField] public Transform MuzzleTarget { get; private set; }
        [field: SerializeField] public Transform EjectionTarget { get; private set; }

        private Camera playerCamera;
        public WeaponController WeaponController { get; private set; }
        
        public IFireBehaviour FireBehaviour { get; private set; }
        public IReloadBehaviour ReloadBehaviour { get; private set; }
        
        public event Action OnWeaponFired;
        public event Action<bool> OnWeaponReload;
        public event Action<bool> OnWeaponSetCurrent;

        private void OnValidate() => this.ValidateRefs();

        public void SetCurrent(bool val)
        {
            OnWeaponSetCurrent?.Invoke(val);
        }

        public virtual void Initialize(WeaponData weaponData, Camera cam, WeaponController weaponController)
        {
            WeaponData = weaponData;
            State = new RuntimeWeaponState(this);
            playerCamera = cam;
            this.WeaponController = weaponController;

            FireBehaviour = WeaponData.FireBehaviour.CreateFireBehavior();
            ReloadBehaviour = WeaponData.ReloadBehaviour.CreateReloadBehavior();

            FireBehaviour?.OnInitialize(this);
            ReloadBehaviour?.OnInitialize(this);

            gameObject.SetActive(false);
        }
        
        protected virtual void Update()
        {
            if (State == null || FireBehaviour == null || ReloadBehaviour == null)
                return;
            
            float deltaTime = Time.deltaTime;
            
            State.Tick(deltaTime);
            FireBehaviour.Tick(deltaTime);
            ReloadBehaviour.Tick(deltaTime);
        }

        public void Fire()
        {
            Vector3 dir = this.GetBulletDirection(Rig.shootPoint) + playerCamera.transform.forward;
            
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject bulletInstance = spawnManager.InstantiateAndSpawn(
                WeaponData.BulletData.BulletPrefab, 
                NetworkManager.Singleton.LocalClientId, 
                true, 
                true,
                false,
                Rig.shootPoint.position,
                Quaternion.LookRotation(dir, Vector3.up));

            if (bulletInstance.TryGetComponent(out Bullet bullet))
            {
                bool shouldCenter = WeaponData.BulletData.ItemData.Data.ItemId != "ShotgunAmmo";
                bullet.Fire(Rig.shootPoint.position, dir, WeaponData.BulletData, WeaponController.DamageMultiplier, shouldCenter);
            }
            else
            {
                Debug.LogWarning($"Could not get component {nameof(DefaultBullet)} from bullet.", bullet);
            }
        }

        public virtual void OnShootInput(InputAction.CallbackContext context)
        {
            FireBehaviour?.OnShootInput(context);
        }

        public virtual void OnReloadInput(InputAction.CallbackContext context)
        {
            ReloadBehaviour?.OnReloadInput(context);
        }

        public Vector3 GetBulletDirection(Transform shootPoint)
        {
            int shotIndex = FireBehaviour.ConsecutiveShots;
            int patternShots = Mathf.Max(WeaponData.RecoilPatternShots, 1);
            
            // Get current pattern index
            int indexInPattern = Mathf.Min(shotIndex, patternShots - 1);
            float t = patternShots > 1 ? (float)indexInPattern / (patternShots - 1) : 0f;
            
            // Base pattern in degrees from curves
            float baseXDeg = -WeaponData.RecoilPatternX?.Evaluate(t) ?? 0f;
            float baseYDeg = WeaponData.RecoilPatternY?.Evaluate(t) ?? 0f;
            
            // Scale with overall dispersion multiplier
            baseXDeg *= WeaponData.CurveMultiplier;
            baseYDeg *= WeaponData.CurveMultiplier;
            
            // Small random jitter around pattern
            float jitterX = Random.Range(WeaponData.BulletDispersionX.x, WeaponData.BulletDispersionX.y) * WeaponData.RandomnessMultiplier;
            float jitterY = Random.Range(WeaponData.BulletDispersionY.x, WeaponData.BulletDispersionY.y) * WeaponData.RandomnessMultiplier;

            float angleX = (baseXDeg + jitterX) * Mathf.Deg2Rad;
            float angleY = (baseYDeg + jitterY) * Mathf.Deg2Rad;
            
            // Get origin directions
            Vector3 forward = shootPoint.forward;
            Vector3 right = shootPoint.right;
            Vector3 up = shootPoint.up;

            Vector3 offset = right * Mathf.Tan(angleX) + up * Mathf.Tan(angleY);

            return (forward + offset).normalized;
        }

        public void RequestOnWeaponFired() => OnWeaponFired?.Invoke();

        public void RequestOnWeaponReloaded(bool isReloading) => OnWeaponReload?.Invoke(isReloading);
    }
}