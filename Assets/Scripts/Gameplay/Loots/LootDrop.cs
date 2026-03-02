using System;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Loots
{
    public class LootDrop : NetworkBehaviour, ILootable, IPoolInstanceListener
    {
        [SerializeField] private Collider boundsCollider;
        private ItemData itemData;
        private bool hasBeenLooted;

        public string LootId => itemData.ItemId;
        Vector3 ILootable.LootPosition => transform.position.Add(y: 0.5f);

        public ItemData GetItemData()
        {
            return itemData;
        }

        public void Initialize(ItemData itemData)
        {
            this.itemData = itemData;
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 2f))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    boundsCollider.enabled = true;
                }
            }
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
            if (IsOwner)
            {
                GetComponent<NetworkObject>().Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public IPool Pool { get; private set; }
        public void OnSpawn(IPool pool)
        {
            Pool = pool;
        }

        public void OnDespawn(IPool pool)
        {
            boundsCollider.enabled = false;
        }
    }
}