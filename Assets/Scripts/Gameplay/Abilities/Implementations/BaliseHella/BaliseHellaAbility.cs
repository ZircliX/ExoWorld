using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class BaliseHellaAbility : BaseAbility<BaliseHellaData>
    {
        private BaliseHella balise;
        
        public BaliseHellaAbility(BaliseHellaData data, GameObject owner) : base(data, owner) { }

        protected override void OnBegin()
        {
            Vector3 forward = Owner.transform.forward;
            balise = Object.Instantiate(Data.BalisePrefab, Owner.transform.position + forward * 3.5f, Quaternion.identity);
            balise.Initialize(Data, forward);
        }

        protected override void OnTick(float deltaTime)
        {
            
        }

        protected override void OnEnd()
        {
            Debug.LogError("Stop Hella Balise");
            balise.Stop();
            balise = null;
        }
    }
}