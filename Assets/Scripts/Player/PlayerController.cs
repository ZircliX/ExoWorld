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
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
    }
}