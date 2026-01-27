using UnityEngine;

namespace OverBang.ExoWorld.Core.Characters
{
    public class QuickSelectionPhase : SelectionPhase
    {
        public QuickSelectionPhase(SelectionSettings selectionSettings) : base(selectionSettings)
        {
        }

        protected override async Awaitable OnBegin()
        {
            await base.OnBegin();
        }

        protected override async Awaitable OnEnd()
        {
            await base.OnEnd();
        }
    }
}