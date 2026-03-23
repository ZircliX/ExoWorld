using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.IK_Animation
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(20000)]
    public class CustomParentConstraint : MonoBehaviour
    {
        [System.Serializable]
        public struct Profile
        {
            public Vector3 offset;
        }
        
        [SerializeField] private bool rotationConstraint;
        [SerializeField] private bool rotationSmooth;
        [SerializeField] private bool positionConstraint;
        [SerializeField] private bool positionSmooth;
        [SerializeField] private float rotationLerp;
        [SerializeField] private float positionLerp;
        
        [SerializeField, ReadOnly] private Transform target;
        [SerializeField] private Profile targetProfile;

        private void LateUpdate()
        {
            // Debug visualization
            if (target != null)
            {
                Debug.DrawLine(transform.position, target.position, Color.green, Time.deltaTime);
            }
            
            if (rotationConstraint && target != null)
            {
                if (rotationSmooth)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation,rotationLerp * Time.deltaTime);
                }
                else
                {
                    transform.rotation = target.rotation;
                }
            }

            if (positionConstraint && target != null)
            {
                Vector3 targetPosition = new Vector3(
                    target.position.x + targetProfile.offset.x, 
                    target.position.y + targetProfile.offset.y, 
                    target.position.z + targetProfile.offset.z);
                
                if (positionSmooth)
                {
                    float t = Mathf.Clamp01(positionLerp * Time.deltaTime);
                    transform.position = Vector3.Lerp(transform.position, targetPosition, t);
                }
                else
                {
                    transform.position = targetPosition;
                }
            }
        }

        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
        }
    }
}