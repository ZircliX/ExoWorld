using Ami.BroAudio;
using OverBang.ExoWorld.Core.Characters;
using Sirenix.OdinInspector;
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
        
        public bool TryGetLineAtIndex(int index, out CharacterLine line)
        {
            if (index >= 0 && index < Lines.Length)
            {
                line = Lines[index];
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
            [ReadOnly] public int characterCount;
            [ShowIf("isTooLong")] public float delayBetweenLines;
            public SoundID soundID;
            private bool isTooLong;
            
            public void UpdateCharacterCount()
            {
                characterCount = text.Length;
            }

            /// <summary>if text is too long, show "delay between lines"</summary> <param name="value"></param>
            public void UpdateCondition(bool value)
            {
                isTooLong = value;
            }
        }
    }
}