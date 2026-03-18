using OverBang.ExoWorld.Core.GameMode;
using OverBang.ExoWorld.Core.Menus;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Menus
{
    public class PauseMenu : MonoPhaseListener<IGameMode>
    {
        [SerializeField] private PauseMenuUI pauseMenuUI;
        private bool isPaused;
        private SurvivalGameMode gameMode;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            pauseMenuUI.OnEscapeClicked += OnEscapeClicked;
            pauseMenuUI.OnResumeClicked += OnResumeClicked;
            pauseMenuUI.OnMenuClicked += OnMenuClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            pauseMenuUI.OnEscapeClicked -= OnEscapeClicked;
            pauseMenuUI.OnResumeClicked -= OnResumeClicked;
            pauseMenuUI.OnMenuClicked -= OnMenuClicked;
        }

        private void OnEscapeClicked()
        {
            if (!isPaused) return;
            Resume();
        }

        private void OnResumeClicked() => Resume();

        private void OnMenuClicked() => gameMode.End();

        private void Resume()
        {
            pauseMenuUI.Hide();
            isPaused = false;
        }

        public void Open(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                pauseMenuUI.Show();
                isPaused = true;
            }
        }
        
        protected override void OnBegin(IGameMode phase)
        {
            if (phase is SurvivalGameMode survivalGameMode)
                gameMode = survivalGameMode;
        }

        protected override void OnEnd(IGameMode phase)
        {
            if (phase is SurvivalGameMode)
                gameMode = null;
        }
    }
}