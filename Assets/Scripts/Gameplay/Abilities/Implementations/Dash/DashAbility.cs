using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DashAbility : BaseAbility<DashData>
    {
        private PlayerMovement pm;
        
        public DashAbility(DashData data, GameObject owner) : base(data, owner) { }

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