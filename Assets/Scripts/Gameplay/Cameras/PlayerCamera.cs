using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Cameras
{
    [DefaultExecutionOrder(0)]
    public class PlayerCamera : MonoBehaviour
    {
        private Vector2 targetCamVelocity;
        private Vector2 camRotation;

        [SerializeField] private float speed = 150;
        [SerializeField, Range(0,1)] private float xModifier = 1;
        [SerializeField, Range(0,1)] private float yModifier = 1;
        private float sens = 1f;
        [SerializeField] private int yRange = 70;
        [SerializeField] private PlayerMovement pm;
        [SerializeField] private Transform playerHead;

        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Transform cameraRotations;
        
        private bool activeInputs = true;
        
        private void OnEnable()
        {
            GamePlayerManager.Instance.GetLocalPlayer().OnStateChanged += OnStateChange;
        }

        private void OnDisable()
        {
            GamePlayerManager.Instance.GetLocalPlayer().OnStateChanged -= OnStateChange;
        }

        private void OnStateChange(PlayerState state)
        {
            activeInputs = state is not (PlayerState.Down or PlayerState.Dead);

            if (state == PlayerState.Down)
            {
                targetCamVelocity.y = 0;
                targetCamVelocity.x = 0;
            }
        }
        
        private void Update()
        {
            camRotation.x -= targetCamVelocity.x * speed * Time.deltaTime * sens;
            camRotation.y += targetCamVelocity.y * speed * Time.deltaTime * sens;
            camRotation.x = Mathf.Clamp(camRotation.x, -yRange, yRange);
        }

        private void LateUpdate()
        {
            cameraRoot.position = playerHead.position;
            
            // --- Calculate Gravity Alignment ---
            Quaternion localYaw = Quaternion.AngleAxis(camRotation.y, Vector3.up);
            Quaternion localPitch = Quaternion.AngleAxis(-camRotation.x, Vector3.right);

            Vector3 up = -pm.Gravity.Value.normalized;
            Vector3 forward = Vector3.ProjectOnPlane(cameraRoot.forward, up).normalized;

            Quaternion look = Quaternion.LookRotation(forward, up);
            //cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, look, gravityAlignSpeed * Time.deltaTime);
            cameraRoot.rotation = look;
            
            Quaternion rot = localYaw * localPitch;
            cameraRotations.localRotation = rot;
        }

        public void OnLookX(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            targetCamVelocity.y = context.ReadValue<float>() * xModifier;
        }

        public void OnLookY(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            targetCamVelocity.x = context.ReadValue<float>() * yModifier;
        }
        
        public void SetSens(float value)
        {
            sens = value;
        }
    }
}