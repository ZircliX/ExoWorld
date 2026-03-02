using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [System.Serializable]
    public class ItemData
    {
        [field: SerializeField] public string ItemId { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public ItemRarity Rarity { get; private set; }
        public int Quantity { get; private set; } = 0;

        public ItemData(string id, string name, int qty = 1, Sprite itemIcon = null, 
            string desc = "", ItemRarity itemRarity = ItemRarity.Commun)
        {
            ItemId = id;
            ItemName = name;
            Icon = itemIcon;
            Description = desc;
            Rarity = itemRarity;
            
            Quantity = qty;
        }

        public ItemData Clone()
        {
            return new ItemData(ItemId, ItemName, Quantity, Icon, Description, Rarity);
        }

        public void SetQuantity(int newQuantity)
        {
            Quantity = Mathf.Max(0, newQuantity);
        }

        public void AddQuantity(int amount)
        {
            Quantity += amount;
        }
    }
    
    public enum ItemRarity
    {
        Commun,
        Rare,
        Légendaire
    }
}