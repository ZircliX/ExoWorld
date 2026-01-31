using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [System.Serializable]
    public class ItemData
    {
        [SerializeField] private string itemId;
        [SerializeField] private string itemName;
        [SerializeField] private int quantity;
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;
        [SerializeField] private ItemRarity rarity;

        public string ItemId => itemId;
        public string ItemName => itemName;
        public int Quantity => quantity;
        public Sprite Icon => icon;
        public string Description => description;
        public ItemRarity Rarity => rarity;

        public ItemData(string id, string name, int qty = 1, Sprite itemIcon = null, 
            string desc = "", ItemRarity itemRarity = ItemRarity.Common)
        {
            itemId = id;
            itemName = name;
            quantity = qty;
            icon = itemIcon;
            description = desc;
            rarity = itemRarity;
        }

        public ItemData Clone()
        {
            return new ItemData(itemId, itemName, quantity, icon, description, rarity);
        }

        public void SetQuantity(int newQuantity)
        {
            quantity = Mathf.Max(0, newQuantity);
        }

        public void AddQuantity(int amount)
        {
            quantity += amount;
        }
    }
    
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}