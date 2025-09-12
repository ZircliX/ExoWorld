using ZTools.RewardSystem.Core;
using ZTools.RewardSystem.Sample.Data;

namespace ZTools.RewardSystem.Sample
{
    public class ItemRewardProcessor : RewardProcessor<InventoryItemRewardData>
    {
        protected override bool TryProcess(InventoryItemRewardData reward)
        {
            RewardManager.LogProvider.Log(this, $"Processing inventory item reward: {reward.ItemName} x{reward.Quantity} " +
                                                $"\nWith a Description: {reward.ItemDescription}");
            
            // Process the inventory item reward
            // Add logic to add the item to the player's inventory
            
            return true;
        }
    }
}