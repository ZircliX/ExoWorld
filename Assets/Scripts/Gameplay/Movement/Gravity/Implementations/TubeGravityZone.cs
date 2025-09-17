using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Movement
{
    [RequireComponent(typeof(MeshCollider)), AddComponentMenu("OverBang/Gravity/Tube")]
    public class TubeGravityZone : GravityZone
    {
        [SerializeField, Self] private MeshCollider mc;
        
        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        {
            Vector3 A = transform.position + transform.up;
            Vector3 B = transform.position - transform.up;
            
            Vector3 AB = B - A;
            Vector3 AP = receiver.Position - A;

            float magnitudeAB = AB.sqrMagnitude;
            float dotProduct = Vector3.Dot(AP, AB);
            float t = dotProduct / magnitudeAB;

            // Le point sur la ligne
            Vector3 pointOnLine = A + t * AB;
            return pointOnLine - receiver.Position;
        }
    }
}