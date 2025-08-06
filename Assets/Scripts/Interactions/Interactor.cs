using UnityEngine;

namespace OverBang.GameName.Interactions
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float maxDistance;
        
        private Interactable currentInteractable;
        
        private void Update()
        {
            Vector3 origin = playerCamera.transform.position;
            Vector3 direction = playerCamera.transform.forward;
            
            Debug.DrawRay(origin, direction * maxDistance, Color.yellow);
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
            {
                Debug.Log(hit.collider.gameObject.name);

                if (hit.collider.TryGetComponent(out Interactable interactible))
                {
                    currentInteractable = interactible;
                    currentInteractable.BubbleActivation(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (currentInteractable != null)
                        {
                            currentInteractable.Interact();
                        }
                    }
                    
                }
                else
                {
                    if (currentInteractable != null)
                    {
                        currentInteractable.BubbleActivation(false);
                        currentInteractable = null;
                    }
                }
            }
        }
    }
}