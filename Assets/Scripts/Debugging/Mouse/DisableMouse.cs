using Helteix.ChanneledProperties.Priorities;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Debugging
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