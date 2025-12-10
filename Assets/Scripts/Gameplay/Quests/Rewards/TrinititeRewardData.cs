using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/RewardData/TrinititeRewardData",  fileName = "TrinititeRewardData")]
    public class TrinititeRewardData : RewardData
    {
        [field : SerializeField] public int TrinititeReward {get; private set;}
        
        
    }
}