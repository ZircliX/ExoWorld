using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class AbilityData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }

        public abstract IAbility CreateInstance(GameObject owner);
    }
}