using System;

namespace OverBang.GameName.Managers
{
    public struct GameMode
    {
        public enum GameModeType
        {
            Solo,
            Multiplayer
        }
        
        public GameModeType CurrentGameMode { get; private set; }
        
        public event Action<GameModeType> OnGameModeChanged;

        public void SetGameMode(GameModeType gameMode)
        {
            if (CurrentGameMode == gameMode) return;
            
            CurrentGameMode = gameMode;
            OnGameModeChanged?.Invoke(gameMode);
        }
    }
}