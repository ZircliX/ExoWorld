using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Core.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class CraftTableUI : NavigablePanel
    {
        [SerializeField] private Button craftButton;
        [SerializeField, Space] private CraftTable craftTable;
        
        [Header("UI Elements")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        
        [Header("Buttons")]
        [SerializeField] private GadgetCraftButton[] gadgetButtons;
        [SerializeField] private ItemCraftButton[] itemButtons;
        
        private ScriptableItemData currentSelectedItem;
        private GadgetData currentSelectedGadget; 
        
        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < gadgetButtons.Length; i++)
            {
                gadgetButtons[i].Initialize(this);
            }

            for (int i = 0; i < itemButtons.Length; i++)
            {
                itemButtons[i].Initialize(this);
            }
        }

        private void OnEnable()
        {
            craftTable.OnCraftEnter += Show;
            craftTable.OnCraftExit += Hide;
            backButton.onClick.AddListener(craftTable.StopLoadoutSelection);
            craftButton.onClick.AddListener(Craft);
        }

        private void OnDisable()
        {
            craftTable.OnCraftEnter -= Show;
            craftTable.OnCraftExit -= Hide;
            backButton.onClick.RemoveListener(craftTable.StopLoadoutSelection);
            craftButton.onClick.RemoveListener(Craft);
        }
        
        public void SelectItem(ScriptableItemData item)
        {
            currentSelectedGadget = null;
            currentSelectedItem = item;

            UpdateInfo();
        }

        public void SelectGadget(GadgetData gadgetData)
        {
            currentSelectedItem = null;
            currentSelectedGadget = gadgetData;
            
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (currentSelectedItem != null)
            {
                ItemData itemData = currentSelectedItem.Data;
                nameText.text = itemData.ItemName;
                descriptionText.text = itemData.Description;
            }
            else if (currentSelectedGadget != null)
            {
                nameText.text = currentSelectedGadget.Name;
                descriptionText.text = currentSelectedGadget.Description;
            }
        }

        private void Craft()
        {
            if (currentSelectedItem != null)
            {
                CraftItem(currentSelectedItem);
            }
            else if (currentSelectedGadget != null)
            {
                CraftGadget(currentSelectedGadget);
            }
        }

        private void CraftItem(ScriptableItemData itemData)
        {
            craftTable.CraftItem(itemData);
        }

        private void CraftGadget(GadgetData gadgetData)
        {
            craftTable.CraftGadget(gadgetData);
        }
    }
}