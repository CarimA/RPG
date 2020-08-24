using System;

namespace PhotoVs.EditorSuite.GameData
{
    public class Map
    {
        public string MaskTextureId { get; set; }
        public string FringeTextureId { get; set; }
        public string Name { get; set; }
        public TimeSpan GameDay { get; set; } // TimeSpan.Zero = no time will pass

    }
}