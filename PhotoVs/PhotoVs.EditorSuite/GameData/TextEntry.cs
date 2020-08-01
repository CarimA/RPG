using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class TextEntry
    {
        public TextEntry()
        {
            LocalisedText = new Dictionary<string, string>();
        }

        public Dictionary<string, string> LocalisedText { get; set; }
    }
}