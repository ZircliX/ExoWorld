using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.GameMode.Players;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public class LootCollector : MonoBehaviour
    {
        [SerializeField] private float autoLootRange = 5f;
        [SerializeField] private bool autoLootEnabled = true;
        [SerializeField] private float autoLootDelay = 0.1f;

        private SphereCollider lootCollider;
        private HashSet<ILootable> availableLoot;
        private float lastLootTime;

        private ResourcesInventory Inventory => GamePlayerManager.Instance.GetLocalPlayer().Inventory;

        public event Action<ItemData> OnLootPickup;

        private void Awake()
        {
            availableLoot = new HashSet<ILootable>();
            lootCollider = GetComponent<SphereCollider>();
            lootCollider.radius = autoLootRange;
            lootCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ILootable lootable))
            {
                availableLoot.Add(lootable);

                // Auto-loot if enabled
                if (autoLootEnabled && Time.time - lastLootTime > autoLootDelay)
                {
                    AttemptLoot(lootable);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ILootable lootable))
            {
                availableLoot.Remove(lootable);
            }
        }

        /// <summary>
        /// Manually loot a specific item
        /// </summary>
        public bool AttemptLoot(ILootable lootable)
        {
            if (lootable == null || !availableLoot.Contains(lootable))
                return false;

            ItemData itemData = lootable.GetItemData();

            // Try to add to inventory
            if (Inventory.AddItem(itemData))
            {
                OnLootPickup?.Invoke(itemData);
                lootable.OnLoot(Inventory);
                availableLoot.Remove(lootable);
                lastLootTime = Time.time;
                return true;
            }

            // Inventory full
            return false;
        }

        /// <summary>
        /// Loot all available items
        /// </summary>
        public int LootAll()
        {
            int looted = 0;
            List<ILootable> lootList = new List<ILootable>(availableLoot);

            foreach (ILootable loot in lootList)
            {
                if (AttemptLoot(loot))
                    looted++;
            }

            return looted;
        }

        /// <summary>
        /// Get all available loot (for UI display)
        /// </summary>
        public IReadOnlyCollection<ILootable> GetAvailableLoot()
        {
            return availableLoot;
        }

        /// <summary>
        /// Manually pick up item by input
        /// </summary>
        public void OnPickupInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (availableLoot.Count > 0)
            {
                ILootable firstLoot = new List<ILootable>(availableLoot)[0];
                AttemptLoot(firstLoot);
            }
        }

        public void SetAutoLootEnabled(bool enabled)
        {
            autoLootEnabled = enabled;
        }

        public bool IsAutoLootEnabled => autoLootEnabled;
        public int AvailableLootCount => availableLoot.Count;
    }
}