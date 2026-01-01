using System.Collections.Generic;
using Helteix.Tools;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class MissileManager
    {
        private readonly List<Missile> missiles;
        private DynamicBuffer<Missile> buffer;
        private int currentLaunchCount;
        private float elapsedSpawnTime;
        
        private float DeltaSec => (data.Duration - data.ActivationTime) / data.MissileCount;
        private readonly DfoData data;
        private readonly Transform baliseTransform;

        public MissileManager(DfoData data, Transform balise)
        {
            baliseTransform = balise;
            missiles = new List<Missile>(data.MissileCount);
            buffer = new DynamicBuffer<Missile>(data.MissileCount);

            elapsedSpawnTime = 0;
            this.data = data;
        }

        public void AddMissile(Missile missile)
        {
            missiles.Add(missile);
            currentLaunchCount++;
        }
        
        public void RemoveMissile(Missile missile) => missiles.Remove(missile);

        public void Tick(float deltaTime)
        {
            elapsedSpawnTime += deltaTime;
        
            int shouldHaveLaunched = Mathf.FloorToInt(elapsedSpawnTime / DeltaSec);
        
            while (currentLaunchCount < shouldHaveLaunched && 
                   currentLaunchCount < data.MissileCount)
            {
                LaunchMissile();
            }
            
            buffer.CopyFrom(missiles);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i].OnTick(deltaTime);
        }

        private void LaunchMissile()
        {
            Vector2 randomCircle = Random.insideUnitCircle * data.DiameterSpawn;

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
                missile.Initialize(data.MissileData, this);
                AddMissile(missile);
            }
        }
    }
}