using Ami.BroAudio;
using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public readonly struct ContextualDialogue
    {
        public readonly string text;
        public readonly SoundID soundID;
        public readonly CharacterData character;
        public readonly bool canBeHeardByEveryone;

        public ContextualDialogue(ContextualDialogueData data, ContextualClip.CharacterLine line)
        {
            canBeHeardByEveryone = data.CanBeHeardByEveryone;
            
            text = line.text;
            soundID = line.SoundID;
            character = line.character;
        }
    }
}