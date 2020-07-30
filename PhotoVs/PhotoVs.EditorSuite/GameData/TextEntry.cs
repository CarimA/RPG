using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class TextEntry
    {
        public Dictionary<string, string> LocalisedText { get; set; }

        public TextEntry()
        {
            LocalisedText = new Dictionary<string, string>();
        }
    }
}