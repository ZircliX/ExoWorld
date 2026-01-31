using System.Collections.Generic;
using DG.Tweening;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetControllerUI : MonoBehaviour
    {
        [field : SerializeField] public List<GadgetUi> gadgetUis { get; private set; }
        [SerializeField, Required] private GadgetController controller;
        
        [SerializeField] private CanvasGroup gadgetWheel;
        [SerializeField] private GadgetUiSelector selector;
        
        
        private void Start()
        {
            gadgetWheel.alpha = 0;
            foreach (GadgetUi gadgetUi in gadgetUis)
            {
                gadgetUi.Initialize(this);
            }
            
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

        private void SelectionBegin()
        {
            ChangeVisibility(true);
            selector.StartSelection();
        }
        
        private void SelectionEnd()
        {
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
                GadgetUi gadgetUi = gadgetUis[i];
                if (player.GadgetInventory.GetGadgetCount(gadgetData, out int amount))
                { 
                    gadgetUi.Refresh(gadgetData);
                }
                else
                {
                    gadgetUi.Clear();
                }
                i++;
            }
        }
        
        
    }
}