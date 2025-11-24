using OverBang.GameName.Gameplay.Interface;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PumpStart : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pump pump;
        [SerializeField, TextArea] private string interactionText = "Press 'F' to start the pump.";

        public string InteractionText => interactionText;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract => !pump.IsStarted && !pump.IsCompleted;


        public void OnPlayerEnter(PlayerInteraction playerInteraction)
        {
        }

        public void OnPlayerExit(PlayerInteraction playerInteraction)
        {
        }

        public void Interact(PlayerInteraction playerInteraction)
        {
            pump.CallStartRepair();
        }
    }
}