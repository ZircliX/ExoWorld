using KBCore.Refs;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace OverBang.GameName.Movement
{
    public class EntityGravity : GravityReceiver
    {
        [SerializeField, Parent] private EntityMovement em;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override Vector3 Position => em.Position;

        protected override void Awake()
        {
            base.Awake();
            Gravity.AddOnValueChangeCallback(ctx => em.Gravity.Write(this, GetGravityDirection()));
        }

        private void Start()
        {
            em.Gravity.AddPriority(this, PriorityTags.Default, GetGravityDirection());
        }
    }
}
