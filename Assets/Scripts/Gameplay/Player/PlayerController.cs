using KBCore.Refs;
using OverBang.GameName.Gameplay.Cameras;
using OverBang.GameName.Gameplay.Movement;
using OverBang.GameName.Metrics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField, Self] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField, Child] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField, Child] public PlayerCamera PlayerCamera { get; private set; }
        [field: SerializeField, Child] public Camera Camera { get; private set; }

        private void OnValidate()
        {
            this.ValidateRefs();
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