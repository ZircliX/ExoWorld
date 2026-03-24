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
        public readonly float timeBetweenLines;
        public readonly int maxSubtitlesTextLenght;
        public readonly SoundID soundID;
        public readonly CharacterData character;
        public readonly bool canBeHeardByEveryone;
        public readonly bool radioEffect;

        public ContextualDialogue(CharacterData characterData, ContextualDialogueData data, ContextualClip.CharacterLine line)
        {
            text = line.text;
            priority = data.Priority;
            voiceLifetime = data.VoiceLifetime;
            subtitleLifetime = line.subtitleLifeTime;
            timeBetweenLines = line.delayBetweenLines;
            maxSubtitlesTextLenght = data.MaxSubtitlesTextLenght;
            soundID = line.soundID;
            character = characterData;
            canBeHeardByEveryone = data.CanBeHeardByEveryone;
            radioEffect = data.RadioEffect;
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