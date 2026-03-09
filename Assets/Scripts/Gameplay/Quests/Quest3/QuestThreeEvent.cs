using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public struct QuestThreeEvent : IGameEvent
    {
        public readonly int pieces;
        public readonly int targetPieces;

        public QuestThreeEvent(int pieces, int targetPieces)
        {
            this.pieces = pieces;
            this.targetPieces = targetPieces;
        }
    }
}