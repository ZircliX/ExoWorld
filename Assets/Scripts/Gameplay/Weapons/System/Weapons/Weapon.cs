using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace OverBang.GameName.Gameplay
{
    public abstract class Weapon : MonoBehaviour
    {
        [field: SerializeField, Required] public WeaponData WeaponData { get; protected set; }
        public RuntimeWeaponState State { get; protected set; }
        public WeaponRig Rig { get; private set; }

        protected Camera playerCamera;
        
        public IFireBehaviour fireBehaviour { get; protected set; }
        public IReloadBehaviour reloadBehaviour { get; protected set; }
        
        public event Action OnWeaponFired;
        public event Action OnWeaponReloaded;
        
        public virtual void Initialize(WeaponData weaponData, Camera cam)
        {
            WeaponData = weaponData;
            State = new RuntimeWeaponState(this);
            playerCamera = cam;

            fireBehaviour = WeaponData.FireBehaviour.CreateFireBehavior();
            reloadBehaviour = WeaponData.ReloadBehaviour.CreateReloadBehavior();

            fireBehaviour?.OnInitialize(this);
            reloadBehaviour?.OnInitialize(this);

            GameObject weaponModel = Instantiate(WeaponData.ModelPrefab, transform);
            Rig = weaponModel.GetComponent<WeaponRig>();
        }
        
        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            
            State.Tick(deltaTime);
            fireBehaviour?.Tick(deltaTime);
            reloadBehaviour?.Tick(deltaTime);
        }

        public abstract void Fire();
        public abstract void FireWithDirection(Vector3 origin, Vector3 direction);

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