using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;
        public BurstGrenadeData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        
        public NetworkSpawnManager spawnManager { get; private set; }
        public LocalGamePlayer player { get; private set; }
        public event Action<IGadget> OnGadgetEnded;
        
        private BurstGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public BurstGrenade(BurstGrenadeData burstGrenadeData)
        {
            Data = burstGrenadeData;
        }
        
        public void Begin(ICaster caster, LocalGamePlayer localPlayer)
        {
            Caster = caster;
            isLaunched = false;
            IsEquiped = true;
            IsCasting = false; 
            player = localPlayer; 
            spawnManager = NetworkManager.Singleton.SpawnManager;
            
            NetworkObject grenade = spawnManager.InstantiateAndSpawn(Data.Prefab, 
                player.ClientID, 
                true,
                true,
                false,
                Caster.CastAnchor.position);

            if (grenade.TryGetComponent(out BurstGrenadeEntity entity))
            {
                grenadeEntity = entity;
                grenadeEntity.FreezeGrenadeRpc(true);
                grenadeEntity.Initialize(Data,this);
            }
        }

        public void Cast(Camera cam)
        {
            isLaunched = true;
            IsCasting = true;
            
            grenadeEntity.Cast(cam.transform.forward);
        }

        public void Tick(float deltaTime)
        {
            if (!isLaunched) return;
            if (grenadeEntity != null)
            {
                grenadeEntity.Tick(deltaTime);
            }
        }

        public void End()
        {
            OnGadgetEnded?.Invoke(this);
            Reset();
        }

        public void Discard()
        {
            grenadeEntity.Discard();
            grenadeEntity = null;
            Reset();
        }

        private void Reset()
        {
            IsEquiped = false;
            IsCasting = false;
            isLaunched = false;
            player = null;
        }
    }
}