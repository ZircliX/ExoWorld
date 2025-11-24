namespace OverBang.GameName.Gameplay.Interface
{
    public interface IInteractable
    {
        string InteractionText { get; }
        int Priority { get; }
        bool CanInteract { get; }
     
        void OnPlayerEnter(PlayerInteraction playerInteraction);
        void OnPlayerExit(PlayerInteraction playerInteraction);
        public void Interact(PlayerInteraction playerInteraction);
    }
}