using UnityEngine;

namespace OverBang.GameName.Core
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