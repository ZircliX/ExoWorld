using MultiGame.Interaction;
using UnityEngine;

namespace Interaction
{
    public class Interactible : MonoBehaviour, IInteractible
    {
        [SerializeField] private GameObject interactingBubble;

        public void BubbleActivation(bool Mode)
        {
            interactingBubble.SetActive(Mode);
        }
        public void Interact()
        {
            interactingBubble.SetActive(true);
            Debug.Log($"Interacting with {gameObject.name}");
        }
        
    }
}