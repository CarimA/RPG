using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class FlagCollection
    {
        public List<Flag> Flags { get; set; }

        public FlagCollection()
        {
            Flags = new List<Flag>();
        }
    }
}