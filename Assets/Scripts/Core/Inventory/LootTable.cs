using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Core.Inventory
{
    [CreateAssetMenu(menuName = "OverBang/Loot/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public struct LootableItemData
        {
            [field: SerializeField, Range(0, 1)] public float DropChance { get; private set; }
            [field: SerializeField] public int MinQuantity { get; private set; }
            [field: SerializeField] public int MaxQuantity { get; private set; }
            [field: SerializeField] public ScriptableItemData Loot { get; private set; }
            [field: SerializeField] public NetworkObject LootPrefab { get; private set; }
            
            public LootableItemData SetProbability(float probability)
            {
                DropChance = probability;
                return this;
            }
        }
        
        [field: SerializeField] public LootableItemData[] LootEntries { get; private set; }
        [field: SerializeField] public bool Equilibrate { get; private set; }
    
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

        public LootableItemData GetLoot(ScriptableItemData loot)
        {
            foreach (LootableItemData entry in LootEntries)
            {
                if (entry.Loot == loot)
                    return entry;
            }

            return default;
        }

        private void OnValidate()
        {
            if (!Equilibrate) return;
            
            using (DictionaryPool<int, float>.Get(out Dictionary<int, float> dictionary))
            {
                float total = 0;
                for (int i = 0; i < LootEntries.Length; i++)
                {
                    LootableItemData clip = LootEntries[i];
                    total += clip.DropChance;
                    
                    dictionary.Add(i, clip.DropChance);
                }

                foreach ((int i, float prob) in dictionary)
                {
                    float normalizedProb = prob / total;
                    LootEntries[i] = LootEntries[i].SetProbability(normalizedProb);
                }
            }
        }
    }
}