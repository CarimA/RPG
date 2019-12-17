using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Utils;
using PhotoVs.Utils.Compression;

namespace PhotoVs.Engine.TiledMaps
{
    public class LayerConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseLayer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            var jo = JObject.Load(reader);
            byte[] buffer = null;
            BaseLayer result;
            switch (jo["type"].Value<string>())
            {
                case "tilelayer":
                    result = new TileLayer();
                    if (jo["encoding"]?.Value<string>() == "base64")
                    {
                        buffer = Convert.FromBase64String(jo["data"].Value<string>());
                        jo["data"].Replace(JToken.FromObject(Array.Empty<int>()));
                    }

                    break;
                case "objectgroup":
                    result = new ObjectLayer();
                    break;
                case "imagelayer":
                    result = new ImageLayer();
                    break;
                default:
                    return null;
            }

            serializer.Populate(jo.CreateReader(), result);

            if (result is TileLayer tl && buffer != null)
                switch (tl.Compression)
                {
                    case null:
                        tl.Data = new int[buffer.Length / sizeof(int)];
                        Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                        break;
                    case "zlib":
                        using (var mStream = new MemoryStream(buffer))
                        {
                            using var stream = new ZlibStream(mStream, CompressionMode.Decompress);
                            var bufferSize = result.Width * result.Height * sizeof(int);
                            Array.Resize(ref buffer, bufferSize);
                            stream.Read(buffer, 0, bufferSize);

                            if (stream.ReadByte() != -1)
                                throw new JsonException();

                            tl.Data = new int[result.Width * result.Height];
                            Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                        }

                        break;
                    case "gzip":
                        using (var mStream = new MemoryStream(buffer))
                        {
                            using var stream = new GZipStream(mStream, CompressionMode.Decompress);
                            var bufferSize = result.Width * result.Height * sizeof(int);
                            Array.Resize(ref buffer, bufferSize);
                            stream.Read(buffer, 0, bufferSize);

                            if (stream.ReadByte() != -1)
                                throw new JsonException();

                            tl.Data = new int[result.Width * result.Height];
                            Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                        }

                        break;
                    default:
                        throw new NotImplementedException($"Compression: {tl.Compression}");
                }


            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}