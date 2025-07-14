using KBCore.Refs;
using RogueLike.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadLink.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField, Self] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField, Child] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField, Child] public PlayerCamera PlayerCamera { get; private set; }
        [field: SerializeField, Parent] public Camera Camera { get; private set; }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void InitializeNetworkSpawn(bool isOwner)
        {
            if (!isOwner)
            {
                Destroy(PlayerInput);
                Destroy(Camera.gameObject);
                Destroy(PlayerCamera.gameObject);
                
                Destroy(PlayerMovement.rb);
                Destroy(PlayerMovement);
            }
        }
    }
}