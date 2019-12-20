using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PhotoVs.Engine.TiledMaps
{
    public static class Utils
    {
        internal static readonly JsonSerializer JsonSerializer = new JsonSerializer();
        internal static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof(Map));

        public static Map ReadJsonMap(this TextReader reader)
        {
            return (Map) JsonSerializer.Deserialize(reader, typeof(Map));
        }

        public static Map ReadTmxMap(this TextReader reader)
        {
            return (Map) XmlSerializer.Deserialize(reader);
        }

        public static void WriteTmxMap(this TextWriter writer, Map map)
        {
            XmlSerializer.Serialize(writer, map);
        }

        private static long GetPosition(this StreamReader reader)
        {
            return reader.BaseStream.Position;
        }

        private static void SetPosition(this StreamReader reader, long position)
        {
            reader.BaseStream.Position = position;
            reader.DiscardBufferedData();
        }

        internal static bool ContainsJson(this StreamReader reader)
        {
            var startPosition = reader.GetPosition();
            for (var c = (char) reader.Read(); c != '{'; c = (char) reader.Read())
                if (c != '\r' && c != '\n' && !char.IsWhiteSpace(c))
                {
                    reader.SetPosition(startPosition);
                    return false;
                }

            reader.SetPosition(startPosition);
            return true;
        }
    }
}