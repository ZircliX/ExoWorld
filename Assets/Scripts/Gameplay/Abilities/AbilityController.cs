using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] private List<AbilityData> startingAbilities;

        private IAbility Primary;
        private IAbility Secondary;

        private void Awake()
        {
            foreach (AbilityData ability in startingAbilities)
            {
                if (ability != null)
                {
                    IAbility abilityInstance = ability.CreateInstance(gameObject);
                    if (Primary == null) Primary = abilityInstance;
                    else if (Secondary == null) Secondary = abilityInstance;
                }
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            
            if (Primary == null) return;
            Primary.OnTick(deltaTime);
            
            if (Secondary == null) return;
            Secondary.OnTick(deltaTime);
        }

        public T GetAbility<T>(IAbility ability) where T : class, IAbility
        {
            if (ability is T typedAbility) return typedAbility;
            return null;
        }
        
        public void UsePrimary(InputAction.CallbackContext context)
        {
            if (!context.performed && CanUseAbility()) return;

            Debug.Log("Primary Ability");
            Primary.Begin();
        }
        
        public void UseSecondary(InputAction.CallbackContext context)
        {
            if (!context.performed && CanUseAbility()) return;

            Debug.Log("Secondary Ability");
            Secondary.Begin();
        }

        private bool CanUseAbility()
        {
            if (Primary is { IsActive: false } && Secondary is { IsActive: false }) return true;
            return false;
        }
    }
}