using Ami.BroAudio;
using OverBang.ExoWorld.Core.Characters;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    [System.Serializable]
    public struct ContextualClip 
    {
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        
        [field: SerializeField, TableList(AlwaysExpanded = true)] 
        public CharacterLine[] Lines { get; private set; }
        
        public bool TryGetLine(out CharacterLine line)
        {
            if (Lines.Length > 0)
            {
                int rnd = Random.Range(0, Lines.Length);
                line = Lines[rnd];
                return true;
            }
            
            line = default;
            return false;
        }
        
        [System.Serializable] 
        public struct CharacterLine
        {
            public float subtitleLifeTime;
            [TextArea(1,20)] public string text;
            [Sirenix.OdinInspector.ReadOnly] public int characterCount;
            public SoundID SoundID;
            
            public void UpdateCharacterCount()
            {
                characterCount = text.Length;
            }
        }
    }
}