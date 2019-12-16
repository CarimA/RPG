using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoVs.TiledMaps
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayerType
    {
        unknown = 0,
        tilelayer,
        objectgroup,
        imagelayer
    }
}