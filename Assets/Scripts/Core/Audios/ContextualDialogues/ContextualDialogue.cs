using System;
using Ami.BroAudio;
using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public readonly struct ContextualDialogue : IEquatable<ContextualDialogue>
    {
        public readonly string text;
        public readonly int priority;
        public readonly float voiceLifetime;
        public readonly float subtitleLifetime;
        public readonly SoundID soundID;
        public readonly CharacterData character;
        public readonly bool canBeHeardByEveryone;

        public ContextualDialogue(CharacterData characterData, ContextualDialogueData data, ContextualClip.CharacterLine line)
        {
            canBeHeardByEveryone = data.CanBeHeardByEveryone;
            voiceLifetime = data.VoiceLifetime;
            subtitleLifetime = line.subtitleLifeTime;
            priority = data.Priority;
            soundID = line.SoundID;
            text = line.text;
            character = characterData;
        }

        public bool Equals(ContextualDialogue other)
        {
            return text == other.text && soundID.Equals(other.soundID) && Equals(character, other.character) && canBeHeardByEveryone == other.canBeHeardByEveryone;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextualDialogue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(text, soundID, character, canBeHeardByEveryone);
        }
    }
}