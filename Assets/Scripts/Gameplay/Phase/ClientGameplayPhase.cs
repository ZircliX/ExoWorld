using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class ClientGameplayPhase : GameplayPhase
    {
        public ClientGameplayPhase(GameplaySettings gameplaySettings) : base(gameplaySettings)
        {
        }

        protected override Awaitable LoadScene()
        {
            throw new System.NotImplementedException();
        }

        protected override Awaitable<LevelManager> CreateLevelManager()
        {
            throw new System.NotImplementedException();
        }
    }
}