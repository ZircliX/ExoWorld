using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Player.PlayerHUD;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetController : NetworkBehaviour, ICaster, IInputReceiver
    {
        [SerializeField] private GadgetControllerUI gadgetControllerUI;
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        
        private GadgetData currentGadgetData;

        public event Action OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
        }


        private void StartGadgetSelection()
        {
            HUD.Instance.SetCursorState(true);
            OnGadgetSelectionBegin?.Invoke();
        }

        public void SelectCurrentGadget(GadgetData data)
        {
            currentGadgetData = data;
        }
        
        private void StopGadgetSelection()
        {
            HUD.Instance.SetCursorState(false);
            OnGadgetSelectionEnd?.Invoke();

            LocalGamePlayer player = GamePlayerManager.Instance.GetLocalPlayer();
            
            if (player.TryGetGadget(currentGadgetData, out IGadget gadget))
            {
                gadget.Begin(this);
            }
        }
        
        private void OnEnd()
        {
            loadoutController.RemoveReceiver(this);
        }

        #region Inputs
        
            public void OnLeftInput(InputAction.CallbackContext context)
            {
                
            }

            public void OnCInput(InputAction.CallbackContext context)
            {
                if (context.performed) StartGadgetSelection();
                if (context.canceled) StopGadgetSelection();
            }
            
        #endregion
    }
}