using Newtonsoft.Json;

namespace PhotoVs.Engine.TiledMaps
{
    public class TileOffset
    {
        [JsonProperty("x")] public int X { get; set; }

        [JsonProperty("y")] public int Y { get; set; }
    }
}