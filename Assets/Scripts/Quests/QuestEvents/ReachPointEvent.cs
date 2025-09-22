using ZTools.ObjectiveSystem.Core.Interfaces;

namespace OverBang.GameName.Quests.QuestEvents
{
    public readonly struct ReachPointEvent : IGameEvent
    {
        public readonly string PointID;

        public ReachPointEvent(string pointID)
        {
            PointID = pointID;
        }
    }
}