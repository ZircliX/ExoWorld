using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Core.Interactions
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerInteraction : MonoBehaviour
    {
        private Camera cam;
        public Camera InteractionCamera
        {
            get
            {
                if (cam == null) cam = Camera.main;
                return cam;
            }
        }

        public event Action<InteractableData> OnNewInteractable;
        public event Action<InteractableData> OnInteractableRemoved;
        public event Action<InteractableData> OnItemPickup;
        public event Action<InteractableData> OnItemDropped;

        public InteractableData CurrentInteractable { get; private set; }
        public InteractableData HeldItem { get; private set; }

        private List<InteractableData> interactables;
        private Dictionary<IInteractable, InteractableData> interactableDataMap;

        private float updateTime = 0.25f;
        private float currentUpdateTime;

        private void Awake()
        {
            interactables = new List<InteractableData>();
            interactableDataMap = new Dictionary<IInteractable, InteractableData>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                if (HeldItem != null && interactable.SupportedInteractions.HasFlag(InteractionType.Pickup))
                {
                    return;
                }
                
                InteractableData data = new InteractableData(interactable);
                interactables.Add(data);
                interactableDataMap[interactable] = data;
                interactable.OnPlayerEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                if (HeldItem != null && interactable.SupportedInteractions.HasFlag(InteractionType.Pickup))
                {
                    return;
                }
                
                if (interactableDataMap.TryGetValue(interactable, out InteractableData data))
                {
                    interactables.Remove(data);
                    interactableDataMap.Remove(interactable);
                    interactable.OnPlayerExit(this);

                    if (CurrentInteractable?.Instance == interactable)
                    {
                        CurrentInteractable = null;
                        OnNewInteractable?.Invoke(null);
                    }

                    OnInteractableRemoved?.Invoke(data);
                }
            }
        }

        private void Update()
        {
            currentUpdateTime += Time.deltaTime;
            if (!(currentUpdateTime < updateTime))
            {
                currentUpdateTime = 0;
                UpdateInteractableData();
                UpdateCurrentInteractable();
            }
        }

        private void UpdateInteractableData()
        {
            foreach (InteractableData data in interactables)
            {
                data.UpdateData();
            }
        }

        private void UpdateCurrentInteractable()
        {
            InteractableData nextInteractable = null;

            if (interactables.Count > 0)
            {
                interactables.Sort((a, b) => a.Instance.Priority.CompareTo(b.Instance.Priority));

                nextInteractable = interactables[^1];
                if (nextInteractable.Instance.Priority < 0)
                    nextInteractable = null;
            }

            if (nextInteractable != CurrentInteractable)
            {
                CurrentInteractable = nextInteractable;
                OnNewInteractable?.Invoke(CurrentInteractable);
            }
        }

        public void RequestUpdateUI(IInteractable interactable)
        {
            if (interactableDataMap.TryGetValue(interactable, out InteractableData data) && data.Instance == interactable)
            {
                OnNewInteractable?.Invoke(data);
            }
        }

        public void Interact(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (CurrentInteractable != null && CurrentInteractable.Instance.CanInteract)
            {
                if (CurrentInteractable.SupportsInteractionType(InteractionType.Interact))
                {
                    CurrentInteractable.Instance.Interact(this);
                }
            }
        }

        public void PickupItem(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (HeldItem != null)
            {
                //DropItem();
                return;
            }

            if (CurrentInteractable != null && CurrentInteractable.Instance.CanInteract &&
                CurrentInteractable.SupportsInteractionType(InteractionType.Pickup))
            {
                PickupInteractable(CurrentInteractable);
            }
        }

        public void PickupInteractable(InteractableData data)
        {
            if (data == null || !data.IsValid())
                return;

            if (HeldItem != null)
            {
                DropItem();
            }

            HeldItem = data;
            HeldItem.IsHeld = true;
            HeldItem.Instance.OnPickup(this);
            OnItemPickup?.Invoke(HeldItem);
        }

        public IInteractable DropItem()
        {
            if (!CanDropCurrentItem())
                return null;

            InteractableData previouslyHeld = HeldItem;
            previouslyHeld.IsHeld = false;
            HeldItem = null;

            previouslyHeld.Instance.OnDrop(this);
            OnItemDropped?.Invoke(CurrentInteractable);
            
            return previouslyHeld.Instance;
        }

        public bool CanDropCurrentItem()
        {
            return HeldItem != null && HeldItem.IsValid() && HeldItem.SupportsInteractionType(InteractionType.Drop);
        }

        public bool IsHoldingItem()
        {
            return HeldItem != null && HeldItem.IsValid();
        }

        public InteractableData GetInteractableData(IInteractable interactable)
        {
            interactableDataMap.TryGetValue(interactable, out InteractableData data);
            return data;
        }

        public bool GetHoldingItemType<T>()
        {
            return HeldItem?.Instance is T;
        }
    }
}