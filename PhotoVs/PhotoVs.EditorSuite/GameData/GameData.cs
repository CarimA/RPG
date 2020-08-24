using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    // goals: completely remove the need for hardcoding any asset or manually moving stuff to the right place
    // ALL IAssetLoader<T>.Get() calls should be done in update/draw loops so that stuff can be automatically
    // updated on a rebuild
    public class GameData
    {
        public GameData()
        {
            GameProperties = new GameProperties();
            Flags = new FlagCollection();

            Scripts = new Dictionary<string, Script>();
            Events = new Dictionary<string, Graph>();
            Strings = new Dictionary<string, TextEntry>();
        }

        public GameProperties GameProperties { get; set; }
        public FlagCollection Flags { get; set; }

        // the IDs are generated at build time and substituted into where they are used
        public Dictionary<string, PackedData> PackedData { get; set; }

        // the IDs are the IDs pulled from their respective DataTreeNodes
        public Dictionary<string, Script> Scripts { get; set; }
        public Dictionary<string, Graph> Events { get; set; }
        public Dictionary<string, TextEntry> Strings { get; set; }
        public Dictionary<string, Map> Maps { get; set; }

        public string SuperTilesetId { get; set; }
        public string OverworldMapId { get; set; }
    }
}