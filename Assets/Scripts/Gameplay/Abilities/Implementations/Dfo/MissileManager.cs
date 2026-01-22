using System;
using System.Collections.Generic;
using Helteix.Tools;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class MissileManager
    {
        private readonly List<Missile> missiles;
        private DynamicBuffer<Missile> buffer;
        private int currentLaunchCount;
        private float elapsedSpawnTime;
        
        private float DeltaSec => data.SpawnDuration / strategyData.MissileCount;
        private readonly DfoStrategyData strategyData;
        private readonly DfoData data;
        private readonly Transform baliseTransform;

        public event Action OnEndMissiles;

        public MissileManager(DfoData data, DfoStrategyData strategyData, Transform balise)
        {
            baliseTransform = balise;
            missiles = new List<Missile>(strategyData.MissileCount);
            buffer = new DynamicBuffer<Missile>(strategyData.MissileCount);

            elapsedSpawnTime = 0;
            this.data = data;
            this.strategyData = strategyData;
        }

        public void AddMissile(Missile missile)
        {
            missiles.Add(missile);
            currentLaunchCount++;
        }
        
        public void RemoveMissile(Missile missile)
        {
            missiles.Remove(missile);
            if (missiles.Count == 0)
            {
                OnEndMissiles?.Invoke();
            }
        }

        public void Tick(float deltaTime)
        {
            elapsedSpawnTime += deltaTime;
        
            int shouldHaveLaunched = Mathf.FloorToInt(elapsedSpawnTime / DeltaSec);
        
            while (currentLaunchCount < shouldHaveLaunched && 
                   currentLaunchCount < strategyData.MissileCount)
            {
                LaunchMissile();
            }
            
            buffer.CopyFrom(missiles);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i].OnTick(deltaTime);
        }

        private void LaunchMissile()
        {
            Vector2 randomCircle = Random.insideUnitCircle * strategyData.DiameterSpawn;

            Vector3 pos = new Vector3(
                randomCircle.x + baliseTransform.position.x,
                data.HeightSpawn,
                randomCircle.y + baliseTransform.position.z);

            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject missileNetwork = spawnManager.InstantiateAndSpawn(
                data.MissilePrefab,
                NetworkManager.Singleton.LocalClientId,
                true,
                true,
                false,
                pos);

            if (missileNetwork.TryGetComponent(out Missile missile))
            {
                missile.Initialize(data.MissileData, this, strategyData.Damage);
                AddMissile(missile);
            }
        }
    }
}