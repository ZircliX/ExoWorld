using OverBang.GameName.Gameplay.Interface;
using UnityEngine;
using UnityUtils;

namespace OverBang.GameName.Gameplay
{
    public class PumpStart : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pump pump;

        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract => !pump.IsStarted && !pump.IsCompleted;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f, x: -1f);

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