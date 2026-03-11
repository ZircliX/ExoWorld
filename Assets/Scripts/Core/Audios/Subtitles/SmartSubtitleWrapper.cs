using System;
using System.Collections.Generic;

namespace OverBang.ExoWorld.Core.Audios
{
    public static class SmartSubtitleWrapper
    {
        private static readonly HashSet<string> orphanWords = new (StringComparer.OrdinalIgnoreCase)
        {
            "un", "une", "le", "la", "les", "de", "du", "des",
            "ce", "cet", "cette", "ces", "mon", "ton", "son",
            "ma", "ta", "sa", "mes", "tes", "ses", "et", "ou",
            "à", "au", "aux", "en", "par", "sur", "sous"
        };

        public static List<string> Split(string text, int maxChars)
        {
            List<string> lines = new();
            string remaining = text.Trim();

            while (remaining.Length > 0)
            {
                if (remaining.Length <= maxChars)
                {
                    lines.Add(remaining);
                    break;
                }

                int cutIndex = remaining.LastIndexOf(' ', maxChars);

                if (cutIndex <= 0)
                {
                    lines.Add(remaining.Substring(0, maxChars));
                    remaining = remaining.Substring(maxChars).Trim();
                    continue;
                }

                string line = remaining.Substring(0, cutIndex).Trim();

                string[] words = line.Split(' ');
                string lastWord = words[words.Length - 1];

                if (orphanWords.Contains(lastWord))
                {
                    int orphanCut = line.LastIndexOf(' ');

                    if (orphanCut > 0)
                        cutIndex = orphanCut;
                }

                lines.Add(remaining.Substring(0, cutIndex).Trim());
                remaining = remaining.Substring(cutIndex).Trim();
            }

            return lines;
        }
    }
}