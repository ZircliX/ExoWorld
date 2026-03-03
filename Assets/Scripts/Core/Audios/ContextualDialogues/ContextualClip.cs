using Ami.BroAudio;
using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    [System.Serializable]
    public struct ContextualClip 
    {
        [field: SerializeField, Range(0,1)]
        public float Probability { get; private set; }
        
        [field: SerializeField]
        public CharacterLine[] lines { get; private set; }

        public bool TryGetLine(CharacterData data, out CharacterLine line)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].character == data)
                {
                    line = lines[i];
                    return true;
                }
            } 
            
            line = default;
            return false;
        }

        public ContextualClip SetProbability(float probability)
        {
            Probability = probability;
            return this;
        }
        
        [System.Serializable] 
        public struct CharacterLine
        {
            public SoundID SoundID;
            public string text;
            public CharacterData character;
        }
    }
}