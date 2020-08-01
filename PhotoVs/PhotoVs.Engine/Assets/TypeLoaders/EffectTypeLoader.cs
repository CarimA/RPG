using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class EffectTypeLoader : TypeLoader<Effect>
    {
        private readonly GraphicsDevice _graphicsDevice;

        public EffectTypeLoader(IAssetLoader assetLoader, GraphicsDevice graphicsDevice) : base(assetLoader)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override Effect Load(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var bytes = ms.ToArray();
            return new Effect(_graphicsDevice, bytes);
        }
    }
}