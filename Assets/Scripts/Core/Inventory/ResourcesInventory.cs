using System;
using System.Collections.Generic;
using System.Linq;
using OverBang.ExoWorld.Core.GameMode.Players;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public class ResourcesInventory
    {
        public static ResourcesInventory Instance => GamePlayerManager.Instance.GetLocalPlayer().Inventory;
        
        private readonly Dictionary<string, ItemData> inventory = new Dictionary<string, ItemData>();
        private readonly List<ItemData> orderedItems = new List<ItemData>(); // Maintains insertion order for UI
        
        public IReadOnlyList<ItemData> Items => orderedItems.AsReadOnly();
        
        public event Action<ItemData> OnItemAdded;
        public event Action<ItemData, int> OnItemRemoved;
        public event Action<ItemData> OnItemQuantityChanged;

        public bool AddItem(ItemData itemData)
        {
            if (itemData is not { Quantity: > 0 })
            {
                Debug.LogWarning("Attempted to add invalid item data");
                return false;
            }

            // Try to stack with existing item
            if (inventory.TryGetValue(itemData.ItemId, out ItemData existingItem))
            {
                existingItem.AddQuantity(itemData.Quantity);
                OnItemQuantityChanged?.Invoke(existingItem);
                return true;
            }

            ItemData newItem = itemData.Clone();
            inventory[itemData.ItemId] = newItem;
            orderedItems.Add(newItem);
            
            OnItemAdded?.Invoke(newItem);
            return true;
        }

        /// <summary>
        /// Add multiple items at once
        /// </summary>
        public bool AddItems(params ItemData[] items)
        {
            foreach (ItemData item in items)
            {
                if (!AddItem(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Remove item from inventory (returns quantity removed)
        /// </summary>
        public int RemoveItem(string itemId, int quantity = 1)
        {
            if (!inventory.TryGetValue(itemId, out ItemData item))
                return 0;

            int removed = Mathf.Min(quantity, item.Quantity);
            item.SetQuantity(item.Quantity - removed);

            if (item.Quantity <= 0)
            {
                inventory.Remove(itemId);
                orderedItems.Remove(item);
            }

            OnItemRemoved?.Invoke(item, removed);
            return removed;
        }

        /// <summary>
        /// Check if inventory has item with minimum quantity
        /// </summary>
        public bool HasItem(string itemId, int minQuantity = 1)
        {
            return inventory.TryGetValue(itemId, out ItemData item) && item.Quantity >= minQuantity;
        }

        /// <summary>
        /// Get specific item data
        /// </summary>
        public ItemData GetItem(string itemId)
        {
            inventory.TryGetValue(itemId, out ItemData item);
            return item;
        }

        /// <summary>
        /// Get total quantity of an item
        /// </summary>
        public int GetItemQuantity(string itemId)
        {
            return inventory.TryGetValue(itemId, out ItemData item) ? item.Quantity : 0;
        }

        /// <summary>
        /// Clear all inventory
        /// </summary>
        public void Clear()
        {
            inventory.Clear();
            orderedItems.Clear();
        }
        
        /// <summary>
        /// Get all items of a specific type/rarity
        /// </summary>
        public List<ItemData> GetItemsByRarity(ItemRarity rarity)
        {
            return orderedItems.Where(i => i.Rarity == rarity).ToList();
        }

        public List<ItemData> GetItemsByName(string name)
        {
            return orderedItems.Where(i => i.ItemName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}