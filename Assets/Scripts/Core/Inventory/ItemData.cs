using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [System.Serializable]
    public struct ItemData : IEquatable<ItemData>
    {
        public enum ItemRarity
        {
            Commun,
            Rare,
            Légendaire
        }
        
        [field: SerializeField] public string ItemId { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public ItemRarity Rarity { get; private set; }
        [field: SerializeField, ReadOnly] public int Quantity { get; private set; }

        [Button] private void ResetQuantity() => Quantity = 0;

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

        public ItemData SetQuantity(int newQuantity)
        {
            Quantity = Mathf.Max(0, newQuantity);
            return this;
        }

        public ItemData AddQuantity(int amount)
        {
            Quantity += amount;
            return this;
        }

        public bool Equals(ItemData other)
        {
            return ItemId == other.ItemId && ItemName == other.ItemName && Equals(Icon, other.Icon) && Description == other.Description && Rarity == other.Rarity && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ItemId, ItemName, Icon, Description, (int)Rarity, Quantity);
        }
    }
}