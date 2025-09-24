using UnityEngine;

namespace OverBang.GameName.Gameplay.Gameplay.Listeners
{
    public abstract class GameplayListener : MonoBehaviour
    {
        protected internal GameplayPhase current;
        
        protected internal abstract void OnInit(GameplayPhase phase);
        protected internal abstract void OnRelease(GameplayPhase phase);
    }
}