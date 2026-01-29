using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public interface ILootable
    {
        Transform transform { get; }
        Vector3 LootPosition => transform.position;
        string LootId { get; }
        ItemData GetItemData();
        void OnLoot(ResourcesInventory inventory);
    }
}