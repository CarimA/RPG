using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.Assets;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class DynamicSpriteFontTypeLoader : ITypeLoader<DynamicSpriteFont>
    {
        private IAssetLoader _assetLoader;
        private readonly int _defaultFontSize;
        private GraphicsDevice _graphicsDevice;

        public DynamicSpriteFontTypeLoader(GraphicsDevice graphicsDevice, IAssetLoader loader, int defaultFontSize)
        {
            _graphicsDevice = graphicsDevice;
            _assetLoader = loader;
            _defaultFontSize = defaultFontSize;
        }

        public DynamicSpriteFont Load(Stream stream)
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            var bytes = memory.ToArray();
            var font = DynamicSpriteFont.FromTtf(bytes, _defaultFontSize);
            return font;
        }
    }
}