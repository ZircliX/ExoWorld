using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/RewardData/TrinititeRewardData",  fileName = "TrinititeRewardData")]
    public class TrinititeRewardData : RewardData
    {
        [field : SerializeField] public int TrinititeReward {get; private set;}
        
        
    }
}