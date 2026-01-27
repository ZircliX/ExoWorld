using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.IK_Animation
{
    [ExecuteInEditMode]
    public class CustomParentConstraint : MonoBehaviour
    {
        [System.Serializable]
        public struct Profile
        {
            public Vector3 offset;
        }
        
        [SerializeField] private bool rotationConstraint;
        [SerializeField] private bool positionConstraint;
        [SerializeField] private float rotationLerp;
        [SerializeField] private float positionLerp;
        
        [SerializeField, ReadOnly] private Transform target;
        [SerializeField] private Profile targetProfile;

        private void LateUpdate()
        {
            if (rotationConstraint && target != null)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation,rotationLerp * Time.deltaTime);
            }

            if (positionConstraint && target != null)
            {
                Vector3 targetPosition = new Vector3(
                    target.position.x + targetProfile.offset.x, 
                    target.position.y + targetProfile.offset.y, 
                    target.position.z + targetProfile.offset.z);
                
                transform.position = Vector3.Lerp(transform.position, targetPosition,positionLerp * Time.deltaTime);
            }
        }

        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
        }
    }
}