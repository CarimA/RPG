using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PhotoVs.Engine.TiledMaps
{
    [XmlRoot("tileset")]
    public class Tileset : ITileset, IXmlSerializable
    {
        [JsonIgnore] //TODO: Add json support
        public Dictionary<int, Frame[]> TileAnimations { get; } = new Dictionary<int, Frame[]>();

        public string Name { get; set; }

        public int FirstGid { get; set; }

        [JsonProperty("image")] public string ImagePath { get; set; }

        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }

        public int Margin { get; set; }
        public int Spacing { get; set; }

        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        public string TransparentColor { get; set; }

        [JsonProperty("tileoffset")] public TileOffset TileOffset { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        [JsonProperty("tileproperties")]
        public Dictionary<int, Dictionary<string, string>> TileProperties { get; } =
            new Dictionary<int, Dictionary<string, string>>();

        public string this[int gid, string property]
        {
            get
            {
                if (gid != 0 && TileProperties.TryGetValue(gid - FirstGid, out var tile) &&
                    tile.TryGetValue(property, out var value))
                    return value;
                return null;
            }
        }

        public Tile this[int gid]
        {
            get
            {
                if (gid == 0)
                    return default;

                var orientation = (TileOrientation)gid & TileOrientation.MaskFlip;

                var columns = Columns;
                var rows = Rows;
                var index = ((int)TileOrientation.MaskID & gid) - FirstGid;
                if (index < 0 || index >= rows * columns)
                    throw new ArgumentOutOfRangeException();

                var row = index / columns;

                return new Tile
                {
                    Top = row * (TileHeight + Spacing) + Margin,
                    Left = (index - row * columns) * (TileWidth + Spacing) + Margin,
                    Width = TileWidth,
                    Height = TileHeight,
                    Orientation = orientation
                };
            }
        }

        public int Columns => (ImageWidth + Spacing - Margin * 2) / (TileWidth + Spacing);
        public int Rows => (ImageHeight + Spacing - Margin * 2) / (TileHeight + Spacing);
        public int TileCount => Columns * Rows;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadTileset(this);
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public static Tileset FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                if (reader.ContainsJson())
                    return (Tileset)Utils.JsonSerializer.Deserialize(reader, typeof(Tileset));
                return (Tileset)new XmlSerializer(typeof(Tileset)).Deserialize(reader);
            }
        }
    }
}