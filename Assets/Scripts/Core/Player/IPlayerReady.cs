namespace OverBang.GameName.Player
{
    public interface IPlayerReady
    {
        bool IsReady { get; }
        void ToggleReady();
    }
}