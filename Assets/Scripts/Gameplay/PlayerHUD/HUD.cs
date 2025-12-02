using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class HUD : SceneService<HUD>
    {
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

        public void SetCursorState(bool state)
        {
            GameController.CursorLockModePriority.Write(this, state ? CursorLockMode.None : CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(this, state);
        }
    }
}