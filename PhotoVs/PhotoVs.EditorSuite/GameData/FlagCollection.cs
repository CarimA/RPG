using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class FlagCollection
    {
        public FlagCollection()
        {
            Flags = new List<Flag>();
        }

        public List<Flag> Flags { get; set; }
    }
}