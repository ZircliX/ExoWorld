using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay.QuestEvents
{
    public struct PumpEvent : IGameEvent
    {
        public float RepairAmount;
        public float MaxRepairAmount;
        
        public PumpEvent(float repairAmount, float maxRepairAmount)
        {
            RepairAmount = repairAmount;
            MaxRepairAmount = maxRepairAmount;
        }
    }
}