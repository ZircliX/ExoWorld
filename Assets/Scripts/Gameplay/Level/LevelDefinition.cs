using System;
using OverBang.GameName.Core.Characters;
using OverBang.Pooling.Resource;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public struct LevelDefinition
    {
        public CharacterData[] characters;

        //[field: SerializeField] public EnemyData[] Enemies { get; private set; }
        
    }
}