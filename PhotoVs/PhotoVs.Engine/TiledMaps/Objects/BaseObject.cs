using Newtonsoft.Json;
using System.Collections.Generic;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    [JsonConverter(typeof(ObjectConverter))]
    public abstract class BaseObject
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("type")] public string ObjectType { get; set; }


        [JsonProperty("visible")] public bool Visible { get; set; }

        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("x")] public int X { get; set; }

        [JsonProperty("y")] public int Y { get; set; }

        [JsonProperty("width")] public int Width { get; set; }

        [JsonProperty("height")] public int Height { get; set; }

        [JsonProperty("rotation")] public float Rotation { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public BaseObject(Dictionary<string, string> properties)
        {
            Properties = properties;
        }
    }
}