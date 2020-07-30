using System.Collections.Generic;
using PhotoVs.EditorSuite.GameData.Events;

namespace PhotoVs.EditorSuite.GameData
{
    public class GameData
    {
        public GameProperties GameProperties { get; set; }
        public FlagCollection Flags { get; set; }

        // the IDs are the IDs pulled from their respective DataTreeNodes
        public Dictionary<string, Script> Scripts { get; set; }
        public Dictionary<string, Graph> Events { get; set; }
        public Dictionary<string, TextEntry> Strings { get; }

        public GameData()
        {
            GameProperties = new GameProperties();
            Flags = new FlagCollection();

            Scripts = new Dictionary<string, Script>();
            Events = new Dictionary<string, Graph>();
            Strings = new Dictionary<string, TextEntry>();
        }
    }
}
