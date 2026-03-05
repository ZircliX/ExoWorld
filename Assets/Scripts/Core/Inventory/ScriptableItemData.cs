using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    [CreateAssetMenu(fileName = "New Item Data", menuName = "OverBang/Loot/Item Data")]
    public class ScriptableItemData : ScriptableObject
    {
        [field: SerializeField] public ItemData ItemData { get; private set; }
        [field: SerializeField] public int DefaultQuantity { get; private set; }
    }
}