using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Health
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private void Update()
        {
            if (!IsOwner)
            {
                Debug.Log("NetworkPlayer: Update called on non-owner client, exiting.", gameObject);
                return;
            }

            // Get the current keyboard state
            var keyboard = Keyboard.current;
            if (keyboard == null) return; // Exit if no keyboard is connected

            Vector3 moveDirection = Vector3.zero;

            // Read key presses directly from the keyboard state
            if (keyboard.wKey.isPressed) moveDirection.z = +1;
            if (keyboard.sKey.isPressed) moveDirection.z = -1;
            if (keyboard.aKey.isPressed) moveDirection.x = -1;
            if (keyboard.dKey.isPressed) moveDirection.x = +1;
        
            float moveSpeed = 5f;
            transform.Translate(moveDirection.normalized * (moveSpeed * Time.deltaTime), Space.World);
        }
    }
}