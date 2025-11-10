using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class ClientGameplayPhase : GameplayPhase
    {
        public ClientGameplayPhase(GameplaySettings gameplaySettings) : base(gameplaySettings)
        {
        }

        protected override async Awaitable LoadScene()
        {
        }

        protected override async Awaitable<LevelManager> CreateLevelManager()
        {
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent<LevelManager>();

            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }

            return LevelManager;
        }
    }
}