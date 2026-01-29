using System;
using System.Collections.Generic;
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
        [field: SerializeField] public Transform CastAnchor { get; private set; }
        
        [SerializeField] private GadgetControllerUI gadgetControllerUI;
        [SerializeField] private List<GadgetData> debugGadgetData;
        
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        
        private GadgetData currentGadgetData;
        private IGadget currentGadget;
        
        private LocalGamePlayer player;

        public event Action OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
            player = GamePlayerManager.Instance.GetLocalPlayer();
            Debug();
        }

        /// <summary>
        /// TODO : DELETE THIS SHIT WHEN CRAFTING TABLE IS HERE
        /// </summary>
        private void Debug()
        {
            foreach (GadgetData gadgetDatas in debugGadgetData)
            {
                player.GadgetInventory.AddGadget(gadgetDatas, GadgetFactory.CreateGadget(gadgetDatas), 10);
            }
        }


        private void StartGadgetSelection()
        {
            HUD.Instance.SetCursorState(true);
            gadgetControllerUI.RefreshGadgetInUI(player);
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
            
            if (player.GadgetInventory.TryGetGadget(currentGadgetData, out IGadget gadget))
            {
                currentGadget = gadget;
                currentGadget.Begin(this);
            }
        }

        private void StartGadget()
        {
            currentGadget.Launch(this);
        }
        
        private void OnEnd()
        {
            loadoutController.RemoveReceiver(this);
        }

        #region Inputs
        
            public void OnLeftInput(InputAction.CallbackContext context)
            {
                if (context.performed && currentGadgetData != null)
                {
                    StartGadget();
                }
            }

            public void OnCInput(InputAction.CallbackContext context)
            {
                if (context.performed) StartGadgetSelection();
                if (context.canceled) StopGadgetSelection();
            }
            
        #endregion
    }
}