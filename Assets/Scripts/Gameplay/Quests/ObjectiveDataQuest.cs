using UnityEngine;
using ZTools.ObjectiveSystem.Core;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public abstract class ObjectiveDataQuest : ObjectiveData
    {
        [field: SerializeField] public RewardData Reward { get; private set; }
    }
}