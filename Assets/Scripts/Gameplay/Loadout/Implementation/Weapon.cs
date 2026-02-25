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

        protected Camera playerCamera;
        public WeaponController WeaponController { get; private set; }
        
        public IFireBehaviour fireBehaviour { get; protected set; }
        public IReloadBehaviour reloadBehaviour { get; protected set; }
        
        public event Action OnWeaponFired;
        public event Action OnWeaponReloaded;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public virtual void Initialize(WeaponData weaponData, Camera cam, WeaponController weaponController)
        {
            WeaponData = weaponData;
            State = new RuntimeWeaponState(this);
            playerCamera = cam;
            this.WeaponController = weaponController;

            fireBehaviour = WeaponData.FireBehaviour.CreateFireBehavior();
            reloadBehaviour = WeaponData.ReloadBehaviour.CreateReloadBehavior();

            fireBehaviour?.OnInitialize(this);
            reloadBehaviour?.OnInitialize(this);

            gameObject.SetActive(false);
        }
        
        protected virtual void Update()
        {
            if (State == null || fireBehaviour == null || reloadBehaviour == null)
                return;
            
            float deltaTime = Time.deltaTime;
            
            State.Tick(deltaTime);
            fireBehaviour.Tick(deltaTime);
            reloadBehaviour.Tick(deltaTime);
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
                bullet.Fire(Rig.shootPoint.position, dir, WeaponData.BulletData);
            }
            else
            {
                Debug.LogWarning($"Could not get component {nameof(DefaultBullet)} from bullet.", bullet);
            }
        }

        public virtual void OnShootInput(InputAction.CallbackContext context)
        {
            fireBehaviour?.OnShootInput(context);
        }

        public virtual void OnReloadInput(InputAction.CallbackContext context)
        {
            reloadBehaviour?.OnReloadInput(context);
        }

        public Vector3 GetBulletDirection(Transform shootPoint)
        {
            int shotIndex = fireBehaviour.ConsecutiveShots;
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

        public void RequestOnWeaponFired()
        {
            OnWeaponFired?.Invoke();
        }

        public void RequestOnWeaponReloaded() => OnWeaponReloaded?.Invoke();
    }
}