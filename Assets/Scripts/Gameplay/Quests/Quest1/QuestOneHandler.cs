using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestOneHandler : ObjectiveHandler<QuestOneData, QuestOneEvent>
    {
        private QuestOneData data;
        
        protected override ObjectiveProgression CalculateProgression(QuestOneData objectiveOneData, QuestOneEvent gameEvent)
        {
            data = objectiveOneData;
            return new ObjectiveProgression(gameEvent.RepairAmount, gameEvent.MaxRepairAmount);
        }

        protected override void ObjectiveCompleted()
        {
            if (data.Reward is TrinititeRewardData trinititeRewardData)
            {
                ResourcesInventory inventory = GamePlayerManager.Instance.GetLocalPlayer().Inventory;
                inventory.AddItem(trinititeRewardData.TrinititeData, trinititeRewardData.RewardQuantity);
            }
        }
    }
}