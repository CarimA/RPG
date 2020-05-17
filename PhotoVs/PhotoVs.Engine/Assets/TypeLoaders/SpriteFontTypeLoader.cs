using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class SpriteFontTypeLoader : ITypeLoader<SpriteFont>
    {
        private readonly IAssetLoader _assetLoader;
        private readonly GraphicsDevice _graphicsDevice;

        public SpriteFontTypeLoader(GraphicsDevice graphicsDevice, IAssetLoader loader)
        {
            _graphicsDevice = graphicsDevice;
            _assetLoader = loader;
        }

        public SpriteFont Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var fontData = reader.ReadToEnd();
            var font = BMFontLoader.LoadXml(fontData,
                name => new TextureWithOffset(_assetLoader.GetAsset<Texture2D>(name)));

            return font;
        }
    }
}