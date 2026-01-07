using UnityEngine;

namespace OverBang.GameName.Core
{
    public abstract class AbilityData : ScriptableObject
    {
        [field: Header("General Data")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: Header("Specific Data")]
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
       
        [field: Header("Strategy Data")]
        [field: Space]
        [field: SerializeReference] public IAbilityStrategyData MainData { get; private set; }
        [field: Space]
        [field: SerializeReference] public IAbilityStrategyData AugmentData1 { get; private set; }
        [field: Space]
        [field: SerializeReference] public IAbilityStrategyData AugmentData2 { get; private set; }
    }
}