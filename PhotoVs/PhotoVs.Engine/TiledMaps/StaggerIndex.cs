using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PhotoVs.Engine.TiledMaps
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StaggerIndex : byte
    {
        None = 0,
        [EnumMember(Value = "odd")] odd,
        [EnumMember(Value = "even")] even
    }
}