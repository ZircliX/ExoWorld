using OverBang.ExoWorld.Core.Inventory;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class ItemCraftButton : CraftButton
    {
        [SerializeField] private ScriptableItemData itemData;

        protected override void Enable()
        {
            text.text = itemData.Data.ItemName;
        }

        protected override void Disable()
        {
        }

        protected override void OnButtonClicked()
        {
            craftTableUI.SelectItem(itemData);
        }

        public override void Initialize(CraftTableUI ui)
        {
            craftTableUI = ui;
        }
    }
}