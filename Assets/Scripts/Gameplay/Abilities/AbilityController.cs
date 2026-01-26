using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AbilityController : MonoBehaviour, IPlayerComponent, ICaster
    {
        public PlayerController Controller { get; set; }
        public Vector3 Forward => pm.CameraController.transform.forward;
        private PlayerMovement pm;
        
        private IAbility primary;
        private IAbility secondary;

        private void Awake()
        {
            pm = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            
            if (primary is null) return;
            primary.Tick(deltaTime);
            
            if (secondary is null) return;
            secondary.Tick(deltaTime);
        }
        
        public void UsePrimary(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            UseAbility(primary);
        }
        
        public void UseSecondary(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            UseAbility(secondary);
        }

        private void UseAbility(IAbility ability)
        {
            if (!CanUseAbility(ability)) return;
            ability.Begin();
        }

        private bool CanUseAbility(IAbility ability)
        {
            bool condition = ability is { CanBeUsed: true };
            return condition;
        }

        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;

            //Initialize abilities
            if (context.playerCharacterData.PrimaryAbility != null)
            {
                primary = context.playerCharacterData.PrimaryAbility.CreateAbilityFor(this);
            }

            if (context.playerCharacterData.SecondaryAbility != null)
            {
                secondary = context.playerCharacterData.SecondaryAbility.CreateAbilityFor(this);
            }
        }
    }
}