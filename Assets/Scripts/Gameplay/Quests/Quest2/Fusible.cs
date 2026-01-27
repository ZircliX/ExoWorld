using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Abilities;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Fusible : NetworkBehaviour, IInteractable, IFusible
    {
        [SerializeField] private DetectionArea detectionArea;

        public string InteractionText => "Ramasser le fusible";
        public int Priority => (int)TargetPriority.Medium;
        public bool CanInteract { get; private set; } = true;
        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f, x: -1f);
        public void Interact(PlayerInteraction playerInteraction)
        {
            CanInteract = false;
        }

        private void Awake()
        {
            detectionArea.SetAllowedTags("Player", "LocalPlayer");
        }

        private void OnEnable()
        {
            detectionArea.OnEnter += OnEnter;
            detectionArea.OnExit += OnExit;
        }

        private void OnDisable()
        {
            detectionArea.OnEnter -= OnEnter;
            detectionArea.OnExit -= OnExit;
        }
        
        private void OnEnter(Collider col, object target)
        {
            CanInteract = true;
        }
        
        private void OnExit(Collider col, object target)
        {
            CanInteract = false;
        }
    }
}