using Ami.BroAudio;
using OverBang.ExoWorld.Core.Components;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public abstract class BaseFireBehaviour : IFireBehaviour
    {
        public int ConsecutiveShots { get; protected set; }
        public Weapon Weapon { get; protected set; }
        
        protected float consecutiveShotsValue;
        protected NetworkSpawnManager spawnManager;
        protected LocalGamePlayer localPlayer;

        public virtual void OnInitialize(Weapon weapon)
        {
            Weapon = weapon;
            Weapon.OnWeaponSetCurrent += OnWeaponSetCurrent;
            
            spawnManager = NetworkManager.Singleton.SpawnManager;
            localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
        }

        protected abstract void OnWeaponSetCurrent(bool val);
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
                if (Weapon.State.CurrentBullets <= 0 && !Weapon.State.IsReloading)
                {
                    //Debug.Log("Reload with fire");
                    Weapon.ReloadBehaviour.Reload();
                }
                //Debug.LogWarning($"Could not consume {bulletsToFire} bullet(s).");
                return;
            }
            
            // Fire all bullets
            for (int i = 0; i < bulletsToFire; i++)
            {
                Weapon.Fire();
                Weapon.RequestOnWeaponFired();
            }
            
            HandleVFX();
            BroAudio.Play(Weapon.WeaponData.FireSound);
            
            WeaponRecoilSettings recoil = Weapon.WeaponData.WeaponRecoilSettings;
            Weapon.WeaponController.Controller.CameraController.RecoilFire(recoil.GetRecoil(), recoil.Snappiness, recoil.ReturnSpeed);
            
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

        protected void HandleVFX()
        {
            NetworkObject muzzle = spawnManager.InstantiateAndSpawn(Weapon.WeaponData.MuzzleFlashPrefab,
                localPlayer.ClientID,
                true,
                true,
                false,
                Weapon.MuzzleTarget.position,
                Weapon.MuzzleTarget.rotation);
                
            if (muzzle.TryGetComponent(out ParticleSystemReference muzzleRef))
            {
                muzzleRef.Play();
                DespawnAfterDelay(muzzleRef.GetComponent<NetworkObject>(), 0.5f).Run();
            }
            else
            {
                Debug.LogWarning($"Could not get {nameof(ParticleSystemReference)} from muzzle flash instance.", muzzle);
                muzzleRef.GetComponent<NetworkObject>().Despawn();
            }
                
            NetworkObject casing = spawnManager.InstantiateAndSpawn(Weapon.WeaponData.EmptyCasePrefab,
                localPlayer.ClientID,
                true,
                true,
                false,
                Weapon.EjectionTarget.position,
                Weapon.EjectionTarget.rotation);
                
            if (casing.TryGetComponent(out ParticleSystemReference casingRef))
            {
                casingRef.Play();
                DespawnAfterDelay(casingRef.GetComponent<NetworkObject>(), 5f).Run();
            }
            else
            {
                Debug.LogWarning($"Could not get {nameof(ParticleSystemReference)} from casing instance.", casing);
                casingRef.GetComponent<NetworkObject>().Despawn();
            }
        }
        
        private async Awaitable DespawnAfterDelay(NetworkObject networkObject, float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            if (!networkObject.IsSpawned)
                return;
            networkObject.Despawn();
        }
    }
}