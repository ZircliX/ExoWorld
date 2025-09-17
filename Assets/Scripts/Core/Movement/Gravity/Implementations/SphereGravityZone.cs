using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Movement
{
    [RequireComponent(typeof(SphereCollider)), AddComponentMenu("OverBang/Gravity/Sphere")]
    public class SphereGravityZone : GravityZone
    {
        [SerializeField, Self] private SphereCollider sc;

        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        {
            return transform.position + sc.center - receiver.Position;
        }
    }
}