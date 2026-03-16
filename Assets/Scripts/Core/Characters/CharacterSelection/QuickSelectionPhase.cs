using System.Threading;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Characters
{
    public class QuickSelectionPhase : SelectionPhase
    {
        public QuickSelectionPhase(SelectionSettings selectionSettings, CancellationTokenSource cts) : base(selectionSettings, cts)
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