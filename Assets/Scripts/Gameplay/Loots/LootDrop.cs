using OverBang.ExoWorld.Core.Inventory;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Loots
{
    public class LootDrop : NetworkBehaviour, ILootable
    {
        [SerializeField] private string lootId;
        [SerializeField] private string lootName;
        [SerializeField] private int quantity = 1;
        [SerializeField] private Sprite lootIcon;
        [SerializeField] private string lootDescription;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;

        private bool hasBeenLooted;

        public string LootId => lootId;
        Vector3 ILootable.LootPosition => transform.position.Add(y: 0.5f);

        public ItemData GetItemData()
        {
            return new ItemData(
                lootId,
                lootName,
                quantity,
                lootIcon,
                lootDescription,
                rarity
            );
        }

        public void OnLoot(ResourcesInventory inventorySystem)
        {
            hasBeenLooted = true;
            
            // Trigger pickup effect (animation, sound, etc.)
            OnLootEffect();
            
            // Despawn the loot
            Despawn();
        }

        private void OnLootEffect()
        {
            // Play pickup animation/sound here
            // Particle effects, etc.
        }

        private void Despawn()
        {
            if (IsServer)
            {
                GetComponent<NetworkObject>().Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Helper to create loot at a position
        /// </summary>
        public static void SpawnLoot(Vector3 position, string id, string name, int qty = 1, 
                                     Sprite icon = null, ItemRarity rarity = ItemRarity.Common)
        {
            GameObject lootObj = new GameObject($"Loot_{id}");
            lootObj.transform.position = position;
            
            LootDrop loot = lootObj.AddComponent<LootDrop>();
            loot.lootId = id;
            loot.lootName = name;
            loot.quantity = qty;
            loot.lootIcon = icon;
            loot.rarity = rarity;

            SphereCollider collider = lootObj.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
        }
    }
}