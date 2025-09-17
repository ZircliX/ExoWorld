using UnityEngine;

namespace OverBang.GameName.Core.Characters
{
    [CreateAssetMenu(fileName = "New Agent Data", menuName = "OverBang/Agent Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        [field: SerializeField] public string AgentName { get; private set; }
        [field: SerializeField] public Sprite AgentSprite { get; private set; }
        [field: SerializeField] public CharacterClasses CharacterClass { get; private set; }
    }
}