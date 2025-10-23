using UnityEngine;

namespace OverBang.GameName.Core.Upgrades
{
    [CreateAssetMenu( fileName = "New Upgrade", menuName = "OverBang/Upgrade Data", order = 0)]
    public class UpgradeAsset : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        
        [field: SerializeField] public UpgradeData Data { get; private set; }
    }
}