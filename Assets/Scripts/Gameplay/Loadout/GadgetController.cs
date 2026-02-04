using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetController : NetworkBehaviour, ICaster, IInputReceiver
    {
        [field: SerializeField] public Transform CastAnchor { get; private set; }
        
        [SerializeField] private List<GadgetData> debugGadgetData;
        
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        
        private GadgetData currentGadgetData;
        private IGadget currentGadget;
        
        private LocalGamePlayer player;

        public event Action<LocalGamePlayer> OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
            player = GamePlayerManager.Instance.GetLocalPlayer();
            Debugss();
        }

        /// <summary>
        /// TODO : DELETE THIS SHIT WHEN CRAFTING TABLE IS HERE
        /// </summary>
        private void Debugss()
        {
            foreach (GadgetData gadgetDatas in debugGadgetData)
            {
                player.GadgetInventory.AddGadget(gadgetDatas, GadgetFactory.CreateGadget(gadgetDatas), 10);
            }
        }

        private void Update()
        {
            if (currentGadget is {IsEquiped : true} )
            {
                currentGadget.Tick(Time.deltaTime);
            }
        }

        private bool CanCastGadget()
        {
            return currentGadget is { IsEquiped : true,  IsCasting : false };
        }
        
        
        private void StartGadgetSelection()
        {
            OnGadgetSelectionBegin?.Invoke(player);
        }

        public void SelectCurrentGadget(GadgetData data)
        {
            if (currentGadgetData != null && currentGadget != null)
            {
                DeselectCurrentGadget();
            }
            currentGadgetData = data;
        }
        
        private void DeselectCurrentGadget()
        {
            Debug.Log("DeselectCurrentGadget");
            currentGadget.Discard();
        }
        
        private void StopGadgetSelection()
        {
            OnGadgetSelectionEnd?.Invoke();
            loadoutController.SwitchCameraInputs(true);
            
            if (currentGadgetData != null && player.GadgetInventory.TryGetGadget(currentGadgetData, out IGadget gadget))
            {
                currentGadget = gadget;
                currentGadget.OnGadgetBeingCasted += ReactiveGameplayInputs;
                currentGadget.OnGadgetEnded += OnEnd;
                currentGadget.Begin(this);
            }
            else
            {
                OnEnd();
            }
        }

        private void StartGadget()
        {
            currentGadget.Cast(this);
            player.GadgetInventory.RemoveGadget(currentGadgetData, 1);
        }

        private void ReactiveGameplayInputs()
        {
            loadoutController.ChangeGameplayInputsState(true);
        }
        
        private void OnEnd()
        {
            if (currentGadget != null)
            {
                currentGadget.OnGadgetBeingCasted -= ReactiveGameplayInputs;
                currentGadget.OnGadgetEnded -= OnEnd;
            }
            
            currentGadget = null;
            currentGadgetData = null;
        }

        #region Inputs
        
            public void OnLeftInput(InputAction.CallbackContext context)
            {
                if (context.performed && CanCastGadget())
                {
                    StartGadget();
                    loadoutController.RemoveReceiver(this);
                }
            }

            public void OnRightInput(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    DeselectCurrentGadget();
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