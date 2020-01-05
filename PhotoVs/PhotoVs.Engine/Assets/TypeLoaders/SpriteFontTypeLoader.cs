using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.Assets;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class SpriteFontTypeLoader : ITypeLoader<SpriteFont>
    {
        private GraphicsDevice _graphicsDevice;
        private IAssetLoader _assetLoader;

        public SpriteFontTypeLoader(IAssetLoader loader) //GraphicsDevice graphicsDevice)
        {
            //_graphicsDevice = graphicsDevice;
            _assetLoader = loader;
        }

        public SpriteFont Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var fontData = reader.ReadToEnd();

            var font = BMFontLoader.LoadXml(fontData, 
                name => _assetLoader.GetAsset<Texture2D>(name));

            return font;
            /*using var memory = new MemoryStream();
            stream.CopyTo(memory);
            var bytes = memory.ToArray();

            var bake = TtfFontBaker.Bake(
                bytes,
                16,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.LatinExtendedB,
                });
            var font = bake.CreateSpriteFont(_graphicsDevice);

            return font;*/
        }
    }
}
