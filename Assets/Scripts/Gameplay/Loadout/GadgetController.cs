using System;
using System.Collections.Generic;
using Helteix.Tools;
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
        public bool IsSelecting { get; private set; }
        
        [SerializeField] private List<GadgetData> debugGadgetData;
        
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        
        private GadgetData currentGadgetData;
        private IGadget currentGadget;
        
        private List<IGadget> castedGadgets;
        private DynamicBuffer<IGadget> gadgetBuffer;
        
        private LocalGamePlayer player;

        public event Action<LocalGamePlayer> OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        private Camera cam;
        public Camera InteractionCamera
        {
            get
            {
                if (cam == null) cam = Camera.main;
                return cam;
            }
        }
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            castedGadgets = new List<IGadget>(8);
            gadgetBuffer  = new DynamicBuffer<IGadget>(8);
            this.loadoutController = loadoutController;
            player = GamePlayerManager.Instance.GetLocalPlayer();
            DebugAddGadgets();
        }

        /// <summary>
        /// TODO : DELETE THIS SHIT WHEN CRAFTING TABLE IS HERE
        /// </summary>
        private void DebugAddGadgets()
        {
            foreach (GadgetData gadgetData in debugGadgetData)
            {
                player.GadgetInventory.AddGadget(gadgetData, 10, () => GadgetFactory.CreateGadget(gadgetData));
            }
        }

        private void Update()
        {
            if (castedGadgets.Count > 0)
            {
                gadgetBuffer.CopyFrom(castedGadgets);
                Debug.Log(gadgetBuffer.Length);
                for (int index = 0; index < gadgetBuffer.Length; index++)
                {
                    IGadget gadget = gadgetBuffer[index];
                    gadget.Tick(Time.deltaTime);
                }
            }
        }

        private bool CanCastGadget()
        {
            return currentGadget is { IsEquiped : true,  IsCasting : false };
        }

        public void SelectCurrentGadget(GadgetData data)
        {
            if (data == null)
            {
                ReactiveGameplayInputs();
                loadoutController.RemoveReceiver(this);
                return;
            }
            
            DeselectCurrentGadget();
            
            if (player.GadgetInventory.GetGadgetCount(data, out int amount) && amount > 0)
            {
                currentGadgetData = data;
                
                if (player.GadgetInventory.TryGetGadget(currentGadgetData, out IGadget gadget))
                {
                    currentGadget = gadget;
                    currentGadget.Begin(this);
                    Debug.Log("Gadget Begin !!!! !!!! !!!!!");
                }
                else
                {
                    Debug.LogError("WTF oO j'suis gay ou quoi ?");
                }
            }
        }
        
        private void DeselectCurrentGadget()
        {
            if (currentGadget == null) return;
            
            currentGadget.Discard();
            currentGadget = null;
            currentGadgetData = null;
            
            loadoutController.RemoveReceiver(this);
            Debug.LogError("Gadget Discard !!!! !!!!!");
        }
        
        private void CastGadget()
        {
            player.GadgetInventory.RemoveGadget(currentGadgetData, 1);
            currentGadget.Cast(InteractionCamera);
            currentGadget.OnGadgetEnded += OnGadgetEnded;
            
            castedGadgets.Add(currentGadget);
            currentGadget = null;
            currentGadgetData = null;
            ReactiveGameplayInputs();
        }

        private void OnGadgetEnded(IGadget gadget)
        {
            gadget.OnGadgetEnded -= OnGadgetEnded;
            castedGadgets.Remove(gadget);
        }

        private void StartGadgetSelection()
        {
            IsSelecting = true;
            OnGadgetSelectionBegin?.Invoke(player);
        }
        
        private void StopGadgetSelection()
        {
            IsSelecting = false;
            OnGadgetSelectionEnd?.Invoke();
            loadoutController.SwitchCameraInputs(true);
        }
        
        #region Inputs
        
            private void ReactiveGameplayInputs()
            {
                loadoutController.ChangeGameplayInputsState(true);
            }
        
            public void OnLeftInput(InputAction.CallbackContext context)
            {
                if (context.performed && CanCastGadget() && !IsSelecting)
                {
                    CastGadget();
                    loadoutController.RemoveReceiver(this);
                }
            }

            public void OnRightInput(InputAction.CallbackContext context)
            {
                if (context.performed && !IsSelecting)
                {
                    DeselectCurrentGadget();
                    ReactiveGameplayInputs();
                }
            }

            public void OnCInput(InputAction.CallbackContext context)
            {
                if (context.performed) 
                    StartGadgetSelection();
                if (context.canceled) 
                    StopGadgetSelection();
            }
            
        #endregion
    }
}