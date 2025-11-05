using UnityEngine;

namespace OverBang.GameName.Core.CharacterSelection
{
    public class QuickSelectionPhase : SelectionPhase
    {
        public QuickSelectionPhase(SelectionSettings selectionSettings) : base(selectionSettings)
        {
        }

        public override async Awaitable OnBegin()
        {
            await base.OnBegin();
        }

        public override async Awaitable OnEnd(bool success)
        {
            await base.OnEnd(success);
        }
    }
}