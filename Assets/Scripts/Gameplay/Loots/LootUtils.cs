using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loots
{
    public static class LootUtils
    {
        public static NetworkObject GetDrop(this LootTable lootTable, Vector3 position, Quaternion rotation, ScriptableItemData preferedDrop = null)
        {
            LootTable.LootableItemData scriptableItemData = default;
            if (preferedDrop == null)
            {
                scriptableItemData = lootTable.GetRandomLoot();
            }
            else
            {
                scriptableItemData = lootTable.GetLoot(preferedDrop);
            }

            if (scriptableItemData.LootPrefab == null && scriptableItemData.Loot == null)
                return null;
            
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject networkObject = spawnManager.InstantiateAndSpawn(
                scriptableItemData.LootPrefab, 
                GamePlayerManager.Instance.GetLocalPlayer().ClientID, 
                true, 
                true,
                false,
                position, 
                rotation);

            if (networkObject.TryGetComponent(out LootDrop lootDrop))
            {
                ItemData itemData = scriptableItemData.Loot.Data;
                int newQuantity = Random.Range(scriptableItemData.MinQuantity, scriptableItemData.MaxQuantity + 1);

                itemData.SetQuantity(newQuantity);
                lootDrop.Initialize(itemData);
                
                if (networkObject.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 direction = (Vector3.up + Random.onUnitSphere).normalized;
                    rb.AddForce(direction * 6f, ForceMode.Impulse);
                }
                
                return networkObject;
            }
            
            return null;
        } 
    }
}