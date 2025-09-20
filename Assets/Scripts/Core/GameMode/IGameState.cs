namespace OverBang.GameName.Core.GameMode
{
    public interface IGameState
    {
        string Name { get; }
        
        void Enter();
        void Exit();
    }
}