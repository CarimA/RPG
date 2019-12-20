using System.Collections.Generic;
using Newtonsoft.Json;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class TileObject : BaseObject
    {
        [JsonProperty("gid")] public int Gid { get; set; }

        internal TileObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public TileObject() : base(new Dictionary<string, string>())
        {
        }
    }
}