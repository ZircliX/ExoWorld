using Helteix.ChanneledProperties.Priorities;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Debug.Mouse
{
    public class DisableMouse : MonoBehaviour
    {
        private void Awake()
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High);
            
            GameController.CursorLockModePriority.Write(this, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(this, false);
        }
    }
}