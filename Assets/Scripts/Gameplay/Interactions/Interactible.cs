using OverBang.GameName.Gameplay.Interface;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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