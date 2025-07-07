using UnityEngine;

namespace DeadLink.Gravity.Implementations
{
    [RequireComponent(typeof(BoxCollider)), AddComponentMenu("RogueLike/Gravity/Box")]
    public class BoxGravityZone : GravityZone
    {
        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        {
            return -transform.up;
        }
    }
}