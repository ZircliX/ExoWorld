using KBCore.Refs;
using UnityEngine;

namespace DeadLink.Gravity.Implementations
{
    [RequireComponent(typeof(BoxCollider)), AddComponentMenu("RogueLike/Gravity/Arc")]
    public class ArcGravityZone : GravityZone
    {
        [SerializeField] private float size = 1f;
        [SerializeField] private float width = 1f;
        
        [SerializeField, Self] private BoxCollider sc;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (sc != null)
            {
                sc.size = new Vector3(size, size, width);
                sc.center = new Vector3(size * 0.5f, size * 0.5f, 0);
            }
        }
        
        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        {
            Vector3 direction = receiver.Position - transform.position;
            direction.x = 0;

            return direction;
        }
    }
}