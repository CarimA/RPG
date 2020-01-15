using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    public class DynamicSpriteFontTypeLoader : ITypeLoader<DynamicSpriteFont>
    {
        private GraphicsDevice _graphicsDevice;
        private IAssetLoader _assetLoader;
        private int _defaultFontSize;

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
