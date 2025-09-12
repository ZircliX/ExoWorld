using System;
using KBCore.Refs;
using OverBang.GameName.Cameras;
using OverBang.GameName.Combat;
using OverBang.GameName.Managers;
using OverBang.GameName.Metrics;
using OverBang.GameName.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField, Self] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField, Child] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField, Child] public PlayerCamera PlayerCamera { get; private set; }
        [field: SerializeField, Child] public Camera Camera { get; private set; }
        [field: SerializeField, Child] public Weapon Weapon { get; private set; }

        public event Action OnReadyKeyPressed;
        public event Action<Vector3> OnShootKeyPressed;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                OnReadyKeyPressed?.Invoke();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 direction = Camera.transform.forward;
                Weapon.Shoot(direction);
                
                OnShootKeyPressed?.Invoke(direction);
            }
        }

        public void EnableLocalControls()
        {
            CameraManager.Instance.SwitchToCamera(CameraID.PlayerView);
        }

        public void DisableRemoteControls(Vector3 pos, Quaternion rot)
        {
            Destroy(PlayerInput);
            Destroy(Camera.gameObject);
            Destroy(PlayerCamera.gameObject);

            PlayerMovement.Rb.isKinematic = true;
            PlayerMovement.Rb.interpolation = RigidbodyInterpolation.Interpolate;
            PlayerMovement.enabled = false;

            PlayerMovement.Rb.position = pos;
            PlayerMovement.Rb.rotation = rot;
        }

        public (Vector3 pos, Quaternion rot) CaptureState()
        {
            return (PlayerMovement.Rb.position, PlayerMovement.Rb.rotation);
        }

        public void ApplyNetworkState(Vector3 targetPos, Quaternion targetRot)
        {
            Vector3 currentPos = PlayerMovement.Rb.position;
            Quaternion currentRot = PlayerMovement.Rb.rotation;

            PlayerMovement.Rb.MovePosition(Vector3.Lerp(currentPos, targetPos, 6f));
            PlayerMovement.Rb.MoveRotation(Quaternion.Slerp(currentRot, targetRot, 6f));
        }
    }
}