namespace OverBang.GameName.Core.Characters
{
    public static class CharacterClassExtension
    {
        /// <summary>
        /// Checks if this class matches (is included in) the given mask.
        /// </summary>
        public static bool Matches(this CharacterClasses characterClass, CharacterClasses mask)
        {
            return (mask & characterClass) != CharacterClasses.None;
        }
    }
}