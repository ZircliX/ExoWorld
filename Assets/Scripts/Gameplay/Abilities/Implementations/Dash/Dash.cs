using UnityEngine;

namespace OverBang.GameName.Gameplay.Dash
{
    public class Dash : BaseAbility<DashData>
    {
        private PlayerMovement pm;
        
        public Dash(DashData data, GameObject owner) : base(data, owner) { }

        protected override void OnBegin()
        {
            if (Owner.TryGetComponent(out pm))
            {
                pm.SetDash(true);
            }
        }

        protected override void OnTick(float deltaTime)
        {
            
        }

        protected override void OnEnd()
        {
            if (pm != null)
                pm.SetDash(false);
        }
    }
}