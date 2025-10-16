using ZTools.ObjectiveSystem.Core.Interfaces;

namespace OverBang.GameName.Quests.QuestEvents
{
    public struct RepairEvent : IGameEvent
    {
        public float RepairAmount;
        public float MaxRepairAmount;
        
        public RepairEvent(float repairAmount, float maxRepairAmount)
        {
            RepairAmount = repairAmount;
            MaxRepairAmount = maxRepairAmount;
        }
    }
}