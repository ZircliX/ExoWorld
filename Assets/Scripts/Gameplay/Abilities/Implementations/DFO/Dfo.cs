using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Dfo : BaseAbility<DfoData>
    {
        private float currentDuration;
        private float currentActivationTime;
        
        private float currentDelta;
        private Transform baliseTransform;
        
        private float DeltaSec => Data.Duration / Data.MissileCount;
        
        public Dfo(DfoData data, GameObject owner) : base(data, owner)
        {
            Cooldown = new CooldownComponent(data.Cooldown);
        }

        protected override void OnBegin()
        {
            Debug.Log("Dfo BEGIN");
            currentDuration = Data.Duration;
            currentActivationTime = Data.ActivationTime;
            
            Balise balise = Object.Instantiate(Data.BalisePrefab, Owner.transform.position + Owner.transform.forward * 3.5f, Quaternion.identity);
            balise.Initialize(Data, Owner.transform.forward);
            baliseTransform = balise.transform;
        }

        protected override void OnTick(float deltaTime)
        {
            //Debug.Log("#UPDATE# Dfo UPDATE");
            currentDuration -= deltaTime;
            if (currentDuration <= 0) End();
            
            //Debug.Log($"ActivationTime remaining: {currentActivationTime}");
            if (currentActivationTime > 0)
            {
                currentActivationTime -= deltaTime;
                return;
            }

            Debug.Log("Launching missiles");
            HandleMissiles(deltaTime);
        }

        protected override void OnEnd()
        {
            Debug.Log("Dfo END");
        }
        
        private void HandleMissiles(float deltaTime)
        {
            currentDelta += deltaTime;
            if (currentDelta >= DeltaSec)
            {
                currentDelta = 0;
                LaunchMissile();
            }
        }
        
        private void LaunchMissile()
        {
            Debug.Log(Data);
            Vector2 randomCircle = Random.insideUnitCircle * Data.DiameterSpawn;
            
            Vector3 pos = new Vector3(
                randomCircle.x + baliseTransform.position.x, 
                Data.HeightSpawn, 
                randomCircle.y + baliseTransform.position.z);
            
            Missile missile = 
                Object.Instantiate(Data.MissilePrefab, pos, Quaternion.identity);
            
            missile.Initialize(Data);
        }
    }
}