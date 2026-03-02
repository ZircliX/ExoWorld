using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Loots
{
    public static class LootUtils
    {
        public static NetworkObject GetDrop(this LootTable lootTable, Vector3 position, Quaternion rotation)
        {
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject networkObject = spawnManager.InstantiateAndSpawn(
                lootTable.NetworkPrefab, 
                GamePlayerManager.Instance.GetLocalPlayer().ClientID, 
                true, 
                true,
                false,
                position, 
                rotation);
            Debug.Log("Spawned Loot");

            if (networkObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(100, position.Add(y: -0.25f), 1f, 100, ForceMode.Impulse);
                Debug.Log("Applied explosion force to loot");
            }

            if (networkObject.TryGetComponent(out LootDrop lootDrop))
            {
                LootTable.LootableItemData scriptableItemData = lootTable.GetRandomLoot();
                ItemData itemData = scriptableItemData.Loot.ItemData;
                itemData.SetQuantity(Random.Range(scriptableItemData.MinQuantity, scriptableItemData.MaxQuantity + 1));
                
                lootDrop.Initialize(itemData);
                Debug.Log("Initialized loot drop");
                
                return networkObject;
            }
            
            return null;
        } 
    }
}