using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [System.Serializable]
    public struct ItemData : IEquatable<ItemData>, INetworkSerializable
    {
        public enum ItemRarity
        {
            Commun,
            Rare,
            Légendaire
        }
        
        public string ItemId => itemId;
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public ItemRarity Rarity => rarity;
        public int Quantity => quantity;
        
        [SerializeField] private string itemId;
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;
        [SerializeField] private ItemRarity rarity;
        [SerializeField, ReadOnly] private int quantity;

        [Button] private void ResetQuantity() => quantity = 0;

        public ItemData(string id, string name, int qty = 1, Sprite itemIcon = null, 
            string desc = "", ItemRarity itemRarity = ItemRarity.Commun)
        {
            itemId = id;
            itemName = name;
            icon = itemIcon;
            description = desc;
            rarity = itemRarity;
            
            quantity = qty;
        }

        public ItemData SetQuantity(int newQuantity)
        {
            quantity = Mathf.Max(0, newQuantity);
            return this;
        }

        public ItemData AddQuantity(int amount)
        {
            quantity += amount;
            return this;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref itemId);
            serializer.SerializeValue(ref itemName);
            serializer.SerializeValue(ref description);
            serializer.SerializeValue(ref quantity);

            // Serialize enum as its underlying int
            int rarityEnum = (int)this.rarity;
            serializer.SerializeValue(ref rarityEnum);

            // After deserialization, resolve the Sprite locally from the ID
            /*
            if (serializer.IsReader)
                icon = ItemRegistry.GetIcon(itemId);
            */
        }

        public bool Equals(ItemData other)
        {
            return itemId == other.itemId && itemName == other.itemName && Equals(icon, other.icon) && description == other.description && rarity == other.rarity && quantity == other.quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(itemId, itemName, icon, description, (int)rarity, quantity);
        }
    }
}