using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public struct QuestTwoEvent : IGameEvent
    {
        public readonly int pieces;

        public QuestTwoEvent(int pieces)
        {
            this.pieces = pieces;
        }
    }
}