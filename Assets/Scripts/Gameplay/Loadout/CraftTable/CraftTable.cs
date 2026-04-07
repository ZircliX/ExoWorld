using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Cameras;
using OverBang.ExoWorld.Gameplay.Core.Menus;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class CraftTable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private Transform uiTarget;
        
        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => uiTarget.position;

        public event Action OnCraftEnter;
        public event Action OnCraftExit;
        
        private LocalGamePlayer player;
        private ScriptableItemData currentSelectedItem;
        private GadgetData currentSelectedGadget; 

        public void Interact(PlayerInteraction playerInteraction)
        {
            player = GamePlayerManager.Instance.GetLocalPlayer();
            StartLoadoutSelection();
        }

        private void StartLoadoutSelection()
        {
            CanInteract = false;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.CraftCamera);
            HUD.Instance.ChangeHudState(true);
            OnCraftEnter?.Invoke();
        }

        public void StopLoadoutSelection()
        {
            CanInteract = true;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
            HUD.Instance.ChangeHudState(false);
            OnCraftExit?.Invoke();
        }
        
        public void CraftItem(ScriptableItemData itemData)
        {
            ItemData item = itemData.Data;
            item.SetQuantity(1);
            
            player.Inventory.AddItem(item);
        }

        public void CraftGadget(GadgetData gadgetData)
        {
            player ??= GamePlayerManager.Instance.GetLocalPlayer();
            
            player.GadgetInventory.AddGadget(gadgetData, 1, () => GadgetFactory.CreateGadget(gadgetData));
        }
    }
}