using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [CreateAssetMenu(menuName = "OverBang/Loot/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public struct LootableItemData
        {
            [field: SerializeField] public float DropChance { get; private set; }
            [field: SerializeField] public int MinQuantity { get; private set; }
            [field: SerializeField] public int MaxQuantity { get; private set; }
            [field: SerializeField] public ScriptableItemData Loot { get; private set; }
        }
        
        [field: SerializeField] public LootableItemData[] LootEntries { get; private set; }
        [field: SerializeField] public NetworkObject NetworkPrefab { get; private set; }
    
        public LootableItemData GetRandomLoot()
        {
            float totalChance = 0f;
            foreach (LootableItemData entry in LootEntries)
            {
                totalChance += entry.DropChance;
            }

            float roll = Random.Range(0f, totalChance);
            float accumulated = 0f;
        
            foreach (LootableItemData entry in LootEntries)
            {
                accumulated += entry.DropChance;
                if (roll <= accumulated)
                    return entry;
            }
        
            return LootEntries[^1];
        }
    }
}