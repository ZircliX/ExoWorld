using OverBang.ExoWorld.Core.Inventory;
using Unity.Netcode;
using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
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
            ResourcesInventory.Instance.AddItem(trinititeRewardData.TrinititeData);
        }
    }
}