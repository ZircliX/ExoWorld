namespace OverBang.ExoWorld.Gameplay.Player
{
    public interface IPlayerComponent
    {
        PlayerController Controller { get; set; }
        void OnSync(PlayerRuntimeContext context);
    }
}