using UnityEngine;

namespace OverBang.ExoWorld.Core.Upgrade
{
    
    [CreateAssetMenu(fileName = "Upgrade", menuName = "OverBang/Upgrade")]
    public class UpgradeData : ScriptableObject , IUpgrade
    {
        [field : SerializeField] public string UpgradeName { get; private set;}
        [field : SerializeField] public string UpgradeDesc { get; private set;}
        [field : SerializeField] public UpgradeType UpgradeType { get; private set;}
        [field : SerializeField] public float Bonus { get; private set;}
        [field : SerializeField] public int Level { get; private set;}
        [field : SerializeField] public int Cost { get; private set;}
    }
    
}