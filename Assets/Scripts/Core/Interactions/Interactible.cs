using UnityEngine;

namespace OverBang.GameName.Interactions
{
    public class Interactable : MonoBehaviour, IInteractable
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