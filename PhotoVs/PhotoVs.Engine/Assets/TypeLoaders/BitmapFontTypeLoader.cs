using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics.BitmapFonts;
using PhotoVs.Engine.Graphics.TextureAtlases;
using PhotoVs.Models.Assets;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class BitmapFontTypeLoader : ITypeLoader<BitmapFont>
    {
        private readonly IAssetLoader _loader;

        public BitmapFontTypeLoader(IAssetLoader loader)
        {
            _loader = loader;
        }

        public BitmapFont Load(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(BitmapFontFile));
            var data = (BitmapFontFile)deserializer.Deserialize(stream);

            var assets = new List<string>();

            foreach (var page in data.Pages)
            {
                var assetName = page.File;
                assets.Add(assetName);
            }

            var textures = assets.Select(t => _loader.GetAsset<Texture2D>(t)).ToArray();

            var lineHeight = data.Common.LineHeight;
            var regionCount = data.Chars.Count;
            var regions = new BitmapFontRegion[regionCount];

            for (var r = 0; r < regionCount; r++)
            {
                var c = data.Chars[r];
                var character = c.Id;
                var textureIndex = c.Page;
                var x = c.X;
                var y = c.Y;
                var width = c.Width;
                var height = c.Height;
                var xOffset = c.XOffset;
                var yOffset = c.YOffset;
                var xAdvance = c.XAdvance;
                var textureRegion = new TextureRegion2D(textures[textureIndex], x, y, width, height);
                regions[r] = new BitmapFontRegion(textureRegion, character, xOffset, yOffset, xAdvance);
            }

            var characterMap = regions.ToDictionary(r => r.Character);
            var kerningCount = data.Kernings.Count;

            for (var k = 0; k < kerningCount; k++)
            {
                var c = data.Kernings[k];
                var first = c.First;
                var second = c.Second;
                var amount = c.Amount;

                // Find region
                if (!characterMap.TryGetValue(first, out var region))
                    continue;

                region.Kernings[second] = amount;
            }

            return new BitmapFont(data.Info.CharSet, regions, lineHeight);
        }
    }
}