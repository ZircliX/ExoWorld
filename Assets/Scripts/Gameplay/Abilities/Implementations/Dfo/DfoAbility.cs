using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DfoAbility : BaseAbility<DfoData>
    {
        private float currentActivationTime;
        private MissileManager missileManager;
        
        public DfoAbility(DfoData data, GameObject owner) : base(data, owner) { }

        protected override void OnBegin()
        {
            //Debug.Log("Dfo BEGIN");
            
            currentActivationTime = Data.ActivationTime;
            
            DfoBalise balise = Object.Instantiate(Data.DfoBalisePrefab, Owner.transform.position + Owner.transform.forward * 3.5f, Quaternion.identity);
            balise.Initialize(Data, Owner.transform.forward);
            missileManager = new MissileManager(Data, balise.transform);
        }

        protected override void OnTick(float deltaTime)
        {
            //Debug.Log($"ActivationTime remaining: {currentActivationTime}");
            if (currentActivationTime > 0)
            {
                currentActivationTime -= deltaTime;
                return;
            }
            
            missileManager.Tick(deltaTime);
        }

        protected override void OnEnd()
        {
        }
    }
}