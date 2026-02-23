namespace OverBang.ExoWorld.Gameplay.Player
{
    public interface IPlayerComponent
    {
        PlayerController Controller { get; }
        void OnSync(PlayerRuntimeContext context);
    }
}