using OverBang.ExoWorld.Core.Damage;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    [CreateAssetMenu(fileName = "QuestThreeData", menuName = "OverBang/ObjectiveData/Quest Three")]
    public class QuestThreeData : ObjectiveDataQuest
    {
        [field: SerializeField] public float TargetSporeToKill { get; private set; }
        [field: SerializeField] public float SporeHealth { get; private set; }
        [field: SerializeField] public DamageData GazTickDamage { get; private set; }
        
        public override IObjectiveHandler GetHandler()
        {
            return new QuestThreeHandler();
        }
    }
}