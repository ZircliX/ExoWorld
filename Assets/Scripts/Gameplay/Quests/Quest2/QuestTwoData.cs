using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    [CreateAssetMenu(fileName = "QuestTwo", menuName = "OverBang/ObjectiveData/QuestTwo", order = 1)]
    public class QuestTwoData : ObjectiveDataQuest
    {
        [field: SerializeField] public int TotalPieces { get; private set; } = 4;
        [field: SerializeField] public float InteractionRange { get; private set; } = 4;
        [field: SerializeField] public float CarryingSlowForce { get; private set; } = 0.6f;
        [field: SerializeField] public string InteractionText { get; private set; }
        [field: SerializeField] public string InteractionTextEmpty { get; private set; }

        public override IObjectiveHandler GetHandler()
        {
            return new QuestTwoHandler();
        }
    }
}