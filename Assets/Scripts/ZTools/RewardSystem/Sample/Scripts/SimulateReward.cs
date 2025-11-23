using UnityEngine;
using ZTools.RewardSystem.Core.ZTools.RewardSystem.Core;

namespace ZTools.RewardSystem.Sample.ZTools.RewardSystem.Sample
{
    public class SimulateReward : MonoBehaviour
    {
        [SerializeField] private RewardData rewardData;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RewardManager.ProcessReward(rewardData);
            }
        }
    }
}