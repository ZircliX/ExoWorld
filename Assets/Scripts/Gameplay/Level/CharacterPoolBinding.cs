using OverBang.GameName.Core.Characters;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "NewCharacterPoolBinding", menuName = "OverBang/Pooling/Character Pool Binding")]
    public class CharacterPoolBinding : ScriptableObject
    {
        [field: SerializeField] public CharacterData Character { get; private set; }
        [field: SerializeField] public PoolConfigAsset PoolConfig { get; private set; }
    }
}