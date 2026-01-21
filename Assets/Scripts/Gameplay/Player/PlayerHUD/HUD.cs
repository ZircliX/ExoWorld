using DG.Tweening;
using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.ExoWorld.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.PlayerHUD
{
    public class HUD : SceneService<HUD>
    {
        [SerializeField] private PlayerInput inputs;
        [SerializeField] private CanvasGroup Ath;
        [SerializeField] private float fadeDuration = 0.5f;
        
        private const string GameplayMapName = "Gameplay";
        private const string UIMapName = "UI";
        
        protected override void Activate()
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.Highest, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.Highest, false);
        }

        protected override void Deactivate()
        {
            GameController.CursorLockModePriority.RemovePriority(this);
            GameController.CursorVisibleStatePriority.RemovePriority(this);
        }

        public void ChangeHudState(bool state)
        {
            if (state)
            {
                inputs.actions.FindActionMap(UIMapName).Enable();
                inputs.actions.FindActionMap(GameplayMapName).Disable();
            }
            else
            {
                inputs.actions.FindActionMap(UIMapName).Disable();
                inputs.actions.FindActionMap(GameplayMapName).Enable();
            }
            
            Ath.DOFade(state ? 0 : 1, fadeDuration);
            
            SetCursorState(state);
        }

        public void SetCursorState(bool state)
        {
            GameController.CursorLockModePriority.Write(this, state ? CursorLockMode.None : CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(this, state);
        }
    }
}