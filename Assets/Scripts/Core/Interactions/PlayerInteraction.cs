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

        public event Action<IInteractable> OnNewInteractable; 
        
        public IInteractable CurrentInteractable { get; private set; }
        private List<IInteractable> interactables;

        private void Awake()
        {
            interactables = new List<IInteractable>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                interactables.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                interactables.Remove(interactable);
            }
        }

        private void Update()
        {
            //Sort
            IInteractable nextInteractable = null;
            if (interactables.Count > 0)
            {
                interactables.Sort((a, b) => a.Priority.CompareTo(b.Priority));

                nextInteractable = interactables[^1];
                if (nextInteractable.Priority < 0 || !nextInteractable.CanInteract)
                    nextInteractable = null;
            }
            
            // Update Current
            if (nextInteractable != CurrentInteractable)
            {
                if (CurrentInteractable != null)
                    CurrentInteractable.OnPlayerExit(this);
                
                CurrentInteractable = nextInteractable;
                
                if (CurrentInteractable != null)
                    CurrentInteractable.OnPlayerEnter(this);
                
                OnNewInteractable?.Invoke(CurrentInteractable);
            }
        }

        public void Interact(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            
            if (CurrentInteractable is { CanInteract: true })
            {
                CurrentInteractable.Interact(this);
            }
        }
    }
}