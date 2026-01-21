using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay
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