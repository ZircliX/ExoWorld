using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public struct QuestOneEvent : IGameEvent
    {
        public float RepairAmount;
        public float MaxRepairAmount;
        
        public QuestOneEvent(float repairAmount, float maxRepairAmount)
        {
            RepairAmount = repairAmount;
            MaxRepairAmount = maxRepairAmount;
        }
    }
}