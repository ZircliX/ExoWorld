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
        
        public LocalGamePlayer Player { get; private set; }

        public event Action<LocalGamePlayer> OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        public event Action OnGadgetCasted; 
        
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
            Player = GamePlayerManager.Instance.GetLocalPlayer();
            DebugAddGadgets();
        }

        /// <summary>
        /// TODO : DELETE THIS SHIT WHEN CRAFTING TABLE IS HERE
        /// </summary>
        private void DebugAddGadgets()
        {
            foreach (GadgetData gadgetData in debugGadgetData)
            {
                Player.GadgetInventory.AddGadget(gadgetData, 40, () => GadgetFactory.CreateGadget(gadgetData));
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (castedGadgets.Count > 0)
            {
                gadgetBuffer.CopyFrom(castedGadgets);
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
            
            if (Player.GadgetInventory.GetGadgetCount(data, out int amount) && amount > 0)
            {
                currentGadgetData = data;
                
                if (Player.GadgetInventory.TryGetGadget(currentGadgetData, out IGadget gadget))
                {
                    currentGadget = gadget;
                    //Debug.Log("Gadget Begin !!!! !!!! !!!!!");
                    currentGadget.Begin(this, Player);
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
            //Debug.LogError("Gadget Discard !!!! !!!!!");
        }
        
        private void CastGadget()
        {
            Player.GadgetInventory.RemoveGadget(currentGadgetData, 1);
            currentGadget.Cast(InteractionCamera);
            currentGadget.OnGadgetEnded += OnGadgetEnded;
            
            castedGadgets.Add(currentGadget);
            currentGadget = null;
            currentGadgetData = null;
            ReactiveGameplayInputs();
            
            OnGadgetCasted?.Invoke();
        }

        private void OnGadgetEnded(IGadget gadget)
        {
            gadget.OnGadgetEnded -= OnGadgetEnded;
            castedGadgets.Remove(gadget);
        }

        private void StartGadgetSelection()
        {
            IsSelecting = !loadoutController.OnlyUI;
            OnGadgetSelectionBegin?.Invoke(Player);
        }
        
        private void StopGadgetSelection()
        {
            OnGadgetSelectionEnd?.Invoke();
            IsSelecting = false;
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