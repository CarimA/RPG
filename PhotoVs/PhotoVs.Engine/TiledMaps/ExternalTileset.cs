using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoVs.Engine.TiledMaps
{
    public class ExternalTileset : ITileset
    {
        public string Source { get; set; }

        private Lazy<Tileset> TilesetDetails { get; set; }
        private ITileset Tileset => TilesetDetails.Value;

        public int FirstGid { get; set; }

        public int Columns => Tileset.Columns;

        public int ImageHeight => Tileset.ImageHeight;

        public string ImagePath => Path.IsPathRooted(Tileset.ImagePath)
            ? Tileset.ImagePath
            : Path.Combine(Path.GetDirectoryName(Source), Tileset.ImagePath);

        public int ImageWidth => Tileset.ImageWidth;
        public int Margin => Tileset.Margin;
        public string Name => Tileset.Name;

        public Dictionary<string, string> Properties => Tileset.Properties;

        public int Rows => Tileset.Rows;

        public int Spacing => Tileset.Spacing;

        public int TileCount => Tileset.TileCount;

        public int TileHeight => Tileset.TileHeight;

        public Dictionary<int, Dictionary<string, string>> TileProperties => Tileset.TileProperties;

        public int TileWidth => Tileset.TileWidth;
        public string TransparentColor => Tileset.TransparentColor;

        public TileOffset TileOffset => Tileset.TileOffset;

        public Tile this[int gid] => Tileset[gid];
        public string this[int gid, string property] => Tileset[gid, property];

        public void LoadTileset(Func<ExternalTileset, Tileset> loader)
        {
            TilesetDetails = new Lazy<Tileset>(() =>
            {
                var tileset = loader(this);
                tileset.FirstGid = FirstGid;
                return tileset;
            });
        }
    }
}