using System.Collections.Generic;
using Helteix.Tools;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Dfo : BaseAbility<DfoData>
    {
        private float currentActivationTime;
        private float elapsedSpawnTime;
        private int missilesLaunched;
        private float DeltaSec => (Data.Duration - Data.ActivationTime) / Data.MissileCount;
        
        private Transform baliseTransform;
        private List<Missile> missiles;
        private DynamicBuffer<Missile> missilesBuffer;
        
        public Dfo(DfoData data, GameObject owner) : base(data, owner) { }

        protected override void OnBegin()
        {
            //Debug.Log("Dfo BEGIN");
            
            missiles = new List<Missile>(Data.MissileCount);
            missilesBuffer = new DynamicBuffer<Missile>(Data.MissileCount);
            currentDuration = Data.Duration;
            currentActivationTime = Data.ActivationTime;
            elapsedSpawnTime = 0;
            missilesLaunched = 0;
            
            Balise balise = Object.Instantiate(Data.BalisePrefab, Owner.transform.position + Owner.transform.forward * 3.5f, Quaternion.identity);
            balise.Initialize(Data, Owner.transform.forward);
            baliseTransform = balise.transform;
        }

        protected override void OnTick(float deltaTime)
        {
            //Debug.Log($"ActivationTime remaining: {currentActivationTime}");
            if (currentActivationTime > 0)
            {
                currentActivationTime -= deltaTime;
                return;
            }

            HandleMissiles(deltaTime);
        }

        protected override void OnEnd()
        {
        }

        private void HandleMissiles(float deltaTime)
        {
            elapsedSpawnTime += deltaTime;
        
            int shouldHaveLaunched = Mathf.FloorToInt(elapsedSpawnTime / DeltaSec);
        
            while (missilesLaunched < shouldHaveLaunched && missilesLaunched < Data.MissileCount)
            {
                LaunchMissile();
                missilesLaunched++;
            }
            
            //Handle Missiles
            missilesBuffer.CopyFrom(missiles);
            for (int i = 0; i < missilesBuffer.Length; i++)
            {
                Missile missile = missilesBuffer[i];
                missile.OnTick(deltaTime);
            }
        }
        
        private void LaunchMissile()
        {
            Vector2 randomCircle = Random.insideUnitCircle * Data.DiameterSpawn;
            
            Vector3 pos = new Vector3(
                randomCircle.x + baliseTransform.position.x, 
                Data.HeightSpawn, 
                randomCircle.y + baliseTransform.position.z);
            
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject missileNetwork = spawnManager.InstantiateAndSpawn(
                Data.MissilePrefab,
                NetworkManager.Singleton.LocalClientId,
                true,
                true,
                false,
                pos);

            if (missileNetwork.TryGetComponent(out Missile missile))
            {
                missile.Initialize(Data);
                missile.OnDetonate += OnMissileDetonate;
                missiles.Add(missile);
            }
        }

        private void OnMissileDetonate(Missile missile)
        {
            missiles.Remove(missile);
            missile.OnDetonate -= OnMissileDetonate;
        }
    }
}