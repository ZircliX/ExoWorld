using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.GameName.Gameplay
{
    public class TrinititeMonoRewardProcessor : NetworkRewardProcessor<TrinititeRewardData>
    {
        [SerializeField] private TrinititeRewardData trinititeRewardData;
        
        protected override bool TryProcess(TrinititeRewardData rewardData)
        {
            if (IsOwner)
                ProcessRpc();
            return true;
        }

        [Rpc(SendTo.Everyone)]
        private void ProcessRpc()
        {
            PlayerInventory.ReceiveTrinitite(trinititeRewardData.TrinititeReward);
        }
    }
}