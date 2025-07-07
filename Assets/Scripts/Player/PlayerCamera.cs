using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueLike.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        private Vector2 targetCamVelocity;
        private Vector2 camRotation;

        [SerializeField] private float speed = 150;
        [SerializeField] private float gravityAlignSpeed;
        [SerializeField, Range(0,1)] private float xModifier = 1;
        [SerializeField, Range(0,1)] private float yModifier = 1;
        private float sens = 1f;
        [SerializeField] private int yRange = 70;
        [SerializeField] private PlayerMovement pm;

        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Transform cameraRotations;
        
        private void Update()
        {
            
            camRotation.x -= targetCamVelocity.x * speed * Time.deltaTime * sens;
            camRotation.y += targetCamVelocity.y * speed * Time.deltaTime * sens;
            camRotation.x = Mathf.Clamp(camRotation.x, -yRange, yRange);

            // --- Calculate Gravity Alignment ---
            Quaternion localYaw = Quaternion.AngleAxis(camRotation.y, Vector3.up);
            Quaternion localPitch = Quaternion.AngleAxis(-camRotation.x, Vector3.right);

            Vector3 up = -pm.Gravity.Value.normalized;
            Vector3 forward = Vector3.ProjectOnPlane(cameraRoot.forward, up).normalized;

            Quaternion look = Quaternion.LookRotation(forward, up);
            cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, look, gravityAlignSpeed * Time.deltaTime);
            
            Quaternion rot = localYaw * localPitch;
            cameraRotations.localRotation = rot;
        }

        public void OnLookX(InputAction.CallbackContext context)
        {
            targetCamVelocity.y = context.ReadValue<float>() * xModifier;
        }

        public void OnLookY(InputAction.CallbackContext context)
        {
            targetCamVelocity.x = context.ReadValue<float>() * yModifier;
        }
        
        public void SetSens(float value)
        {
            sens = value;
        }
    }
}