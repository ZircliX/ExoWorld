using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
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