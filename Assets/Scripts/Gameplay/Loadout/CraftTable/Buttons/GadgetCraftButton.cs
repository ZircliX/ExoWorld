using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetCraftButton : CraftButton
    {
        [SerializeField] private GadgetData gadgetData;

        protected override void Enable()
        {
            text.text = gadgetData.Name;
        }

        protected override void Disable()
        {
        }

        protected override void OnButtonClicked()
        {
            craftTableUI.SelectGadget(gadgetData);
        }

        public override void Initialize(CraftTableUI ui)
        {
            craftTableUI = ui;
        }
    }
}