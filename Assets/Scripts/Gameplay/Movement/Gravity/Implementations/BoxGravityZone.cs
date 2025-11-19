using UnityEngine;

namespace OverBang.GameName.Gameplay.Gravity.Implementations
{
    [RequireComponent(typeof(BoxCollider)), AddComponentMenu("OverBang/Gravity/Box")]
    public class BoxGravityZone : GravityZone
    {
        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        {
            return -transform.up;
        }
    }
}