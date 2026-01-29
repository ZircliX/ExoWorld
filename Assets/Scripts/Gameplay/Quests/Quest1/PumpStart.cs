using OverBang.ExoWorld.Core.Interactions;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class PumpStart : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pump pump;

        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract => !pump.IsStarted && !pump.IsCompleted;
        public InteractionType SupportedInteractions => InteractionType.Interact;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f, x: -1f);

        public void Interact(PlayerInteraction playerInteraction)
        {
            pump.CallStartRepair();
        }
    }
}