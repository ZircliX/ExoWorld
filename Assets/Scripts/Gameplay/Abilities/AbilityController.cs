using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class AbilityController : MonoBehaviour, IPlayerComponent
    {
        public PlayerController Controller { get; set; }
        
        private IAbility primary;
        private IAbility secondary;

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
            if (!context.performed || !CanUseAbility(primary)) return;

            //Debug.Log("Primary Ability Input");
            primary.Begin();
        }
        
        public void UseSecondary(InputAction.CallbackContext context)
        {
            if (!context.performed || !CanUseAbility(secondary)) return;

            Debug.Log("Secondary Ability Input");
            secondary.Begin();
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
                primary = context.playerCharacterData.PrimaryAbility.CreateInstance(Controller.PlayerTransform.gameObject);
                Debug.Log("Initialize primary ability");
            }

            if (context.playerCharacterData.SecondaryAbility != null)
            {
                Debug.Log("Initialize secondary ability");
                secondary = context.playerCharacterData.SecondaryAbility.CreateInstance(Controller.PlayerTransform.gameObject);
            }
        }
    }
}