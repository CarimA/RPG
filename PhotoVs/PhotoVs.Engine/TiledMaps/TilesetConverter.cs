using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace PhotoVs.Engine.TiledMaps
{
    public class TilesetConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ITileset);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            ITileset result;
            if (jo["source"] == null)
                result = new Tileset();
            else
                result = new ExternalTileset();

            serializer.Populate(jo.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}