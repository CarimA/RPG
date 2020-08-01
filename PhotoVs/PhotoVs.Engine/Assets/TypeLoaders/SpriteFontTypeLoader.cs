using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class SpriteFontTypeLoader : TypeLoader<SpriteFont>
    {
        private readonly IAssetLoader _assetLoader;
        private readonly GraphicsDevice _graphicsDevice;

        public SpriteFontTypeLoader(IAssetLoader assetLoader, GraphicsDevice graphicsDevice) : base(assetLoader)
        {
            _graphicsDevice = graphicsDevice;
            _assetLoader = assetLoader;
        }

        public override SpriteFont Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var fontData = reader.ReadToEnd();
            var font = BMFontLoader.LoadXml(fontData,
                name => new TextureWithOffset(_assetLoader.Get<Texture2D>(name)));

            return font;
        }
    }
}