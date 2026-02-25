using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public struct QuestTwoEvent : IGameEvent
    {
        public readonly int pieces;
        public readonly int targetPieces;

        public QuestTwoEvent(int pieces, int targetPieces)
        {
            this.pieces = pieces;
            this.targetPieces = targetPieces;
        }
    }
}