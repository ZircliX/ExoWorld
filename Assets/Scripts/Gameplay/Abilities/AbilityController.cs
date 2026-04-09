using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Scene;
using OverBang.ExoWorld.Gameplay.Movement;
using OverBang.ExoWorld.Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AbilityController : MonoBehaviour, IPlayerComponent, ICaster
    {
        public PlayerController Controller { get; private set; }
        [field: SerializeField] public Transform CastAnchor { get; private set; }
        public Vector3 Forward => pm.CameraController.transform.forward;
        private PlayerMovement pm;
        
        private IAbility primary;
        private IAbility secondary;

        private bool activeInputs = true;
        
        public event Action<IAbility, IAbility> OnAbilitiesChanged;
        public event Action<IAbility> OnAbilityUsed;

        private void Awake()
        {
            if (transform.GetChild(1).TryGetComponent(out pm))
            {
                CastAnchor = transform.GetChild(1);
            }
        }
        
        private void OnDestroy()
        {
            Controller.LocalGamePlayer.OnStateChanged -= OnStateChange;
        }

        private void OnStateChange(PlayerState state)
        {
            activeInputs = state is not (PlayerState.Down or PlayerState.Dead);
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
            if (!activeInputs) return;
            if (!CanUseAbility(ability)) return;
            ability.Begin();
            OnAbilityUsed?.Invoke(ability);
        }

        private bool CanUseAbility(IAbility ability)
        {
            bool canBeUsed = ability is { CanBeUsed: true };
            bool scene = SceneLoader.GetCurrentScene().name != GameMetrics.Global.SceneCollection.HubSceneRef.Name;
            return canBeUsed && scene;
        }

        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;
            Controller.LocalGamePlayer.OnStateChanged += OnStateChange;

            //Initialize abilities
            if (context.playerCharacterData.PrimaryAbility != null)
            {
                primary = context.playerCharacterData.PrimaryAbility.CreateAbilityFor(this);
            }

            if (context.playerCharacterData.SecondaryAbility != null)
            {
                secondary = context.playerCharacterData.SecondaryAbility.CreateAbilityFor(this);
            }
            
            OnAbilitiesChanged?.Invoke(primary, secondary);
        }
    }
}