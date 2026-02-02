using System;
using System.Collections.Generic;
using DG.Tweening;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetControllerUI : MonoBehaviour
    {
        [SerializeField, Self] private GadgetUiSelector selector;
        [SerializeField, Required] private GadgetController controller;
        
        [SerializeField, Self] private CanvasGroup gadgetWheel;
        
        [field : SerializeField] public List<GadgetUi> GadgetUis { get; private set; }
        public event Action OnGadgetUiSelectionEnd;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Start()
        {
            gadgetWheel.alpha = 0;
            selector.Initialize(this);
        }

        private void OnEnable()
        {
            controller.OnGadgetSelectionBegin += SelectionBegin;
            controller.OnGadgetSelectionEnd += SelectionEnd;
        }

        private void OnDisable()
        {
            controller.OnGadgetSelectionBegin -= SelectionBegin;
            controller.OnGadgetSelectionEnd -= SelectionEnd;
        }

        private void SelectionBegin(LocalGamePlayer player)
        {
            RefreshGadgetInUI(player);
            ChangeVisibility(true);
            selector.StartSelection();
        }
        
        private void SelectionEnd()
        {
            OnGadgetUiSelectionEnd?.Invoke();
            ChangeVisibility(false);
        }
        
        public void SetCurrentSelectedGadget(GadgetData data)
        {
            controller.SelectCurrentGadget(data);
        }
        
        private void ChangeVisibility(bool visible)
        {
            gadgetWheel.DOFade(visible ? 1 : 0, 0.20f).OnComplete(() =>
            {
                gadgetWheel.interactable = visible;
                gadgetWheel.blocksRaycasts = visible;
            });
        }
        
        public void RefreshGadgetInUI(LocalGamePlayer player)
        {
            int i = 0;
            
            foreach (GadgetData gadgetData in player.GadgetInventory.GadgetDatas)
            {
                GadgetUi gadgetUi = GadgetUis[i];
                if (player.GadgetInventory.GetGadgetCount(gadgetData, out int amount) && amount > 0)
                { 
                    gadgetUi.SetSelectable(true);
                    gadgetUi.Refresh(gadgetData, amount);
                }
                else
                {
                    gadgetUi.SetSelectable(false);
                    gadgetUi.Clear();
                }
                i++;
            }
        }
    }
}